using Microsoft.JSInterop;

namespace WebRtcWasm.Client.WebRtc;

public interface IWebRtcService
{
    event EventHandler<IJSObjectReference>? OnRemoteStreamAcquired;
    Task Join(string signalingChannel);
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