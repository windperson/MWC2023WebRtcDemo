#nullable enable
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;

namespace WebRtcMauiApp.Services.WebRtc;

public class WebRtcService : IWebRtcService
{
    private readonly IJSRuntime _jsRuntime;

    private IJSObjectReference? _jsModule;
    private DotNetObjectReference<WebRtcService>? _jsThis;
    private HubConnection? _hub;
    private string? _signalingHostUrl;
    private string? _signalingChannel;
    public event EventHandler<IJSObjectReference>? OnRemoteStreamAcquired;

    public WebRtcService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }


    public async Task Join(string signalingHostUrl, string signalingChannel, string user)
    {
        if (_signalingChannel != null)
        {
            throw new InvalidOperationException();
        }

        _signalingHostUrl = signalingHostUrl;
        _signalingChannel = signalingChannel;

        var hub = await GetSignalRHub();
        await hub.SendAsync(user, signalingChannel);

        _jsModule = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "/js/WebRtcService.cs.js");
        _jsThis = DotNetObjectReference.Create(this);
        await _jsModule.InvokeVoidAsync("initialize", _jsThis);
    }


    public async Task<IJSObjectReference> StartLocalStream()
    {
        if (_jsModule == null)
        {
            throw new InvalidOperationException();
        }
        var stream = await _jsModule.InvokeAsync<IJSObjectReference>("startLocalStream");
        return stream;
    }

    public async Task Call()
    {
        if (_jsModule == null)
        {
            throw new InvalidOperationException();
        }
        var offerDescription = await _jsModule.InvokeAsync<string>("callAction");
        await SendOffer(offerDescription);
    }

    public async Task Hangup()
    {
        if (_jsModule == null)
        {
            throw new InvalidOperationException();
        }
        await _jsModule.InvokeVoidAsync("hangupAction");

        var hub = await GetSignalRHub();
        await hub.SendAsync("leave", _signalingChannel);

        _signalingChannel = null;

    }

    [JSInvokable]
    public async Task SendOffer(string offer)
    {
        var hub = await GetSignalRHub();
        await hub.SendAsync("SignalWebRtc", _signalingChannel, "offer", offer);
    }

    [JSInvokable]
    public async Task SendAnswer(string answer)
    {
        var hub = await GetSignalRHub();
        await hub.SendAsync("SignalWebRtc", _signalingChannel, "answer", answer);
    }

    [JSInvokable]
    public async Task SendCandidate(string candidate)
    {
        var hub = await GetSignalRHub();
        await hub.SendAsync("SignalWebRtc", _signalingChannel, "candidate", candidate);
    }

    [JSInvokable]
    public async Task SetRemoteStream()
    {
        if (_jsModule == null)
        {
            throw new InvalidOperationException();
        }
        var stream = await _jsModule.InvokeAsync<IJSObjectReference>("getRemoteStream");
        OnRemoteStreamAcquired?.Invoke(this, stream);
    }


    #region private method

    private async Task<HubConnection> GetSignalRHub()
    {
        if (_hub != null)
        {
            return _hub;
        }

        var hub = new HubConnectionBuilder()
            .WithUrl(_signalingHostUrl + "/messagehub")
            .Build();

        hub.On<string, string, string>("SignalWebRtc", async (signalingChannel, type, payload) =>
        {
            if (_jsModule == null) throw new InvalidOperationException();

            if (_signalingChannel != signalingChannel) return;
            switch (type)
            {
                case "offer":
                    await _jsModule.InvokeVoidAsync("processOffer", payload);
                    break;
                case "answer":
                    await _jsModule.InvokeVoidAsync("processAnswer", payload);
                    break;
                case "candidate":
                    await _jsModule.InvokeVoidAsync("processCandidate", payload);
                    break;
            }
        });

        await hub.StartAsync();
        _hub = hub;
        return _hub;
    }


    #endregion
}