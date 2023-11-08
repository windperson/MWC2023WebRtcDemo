#nullable enable
using Microsoft.JSInterop;

namespace WebRtcMauiApp.Services.WebRtc;

public interface IWebRtcService
{
    event EventHandler<IJSObjectReference>? OnRemoteStreamAcquired;
    Task Join(string signalingHostUrl, string signalingChannel, string user);
    Task<IJSObjectReference> StartLocalStream();
    Task Call();
    Task Hangup();

    [JSInvokable]
    Task SendOffer(string offer);
    [JSInvokable]
    Task SendAnswer(string answer);
    [JSInvokable]
    Task SendCandidate(string candidate);
    [JSInvokable]
    Task SetRemoteStream();
}