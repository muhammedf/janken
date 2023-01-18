using Godot;
using Godough;
using Nakama;
using System;
using System.Threading.Tasks;
using static System.Threading.Tasks.TaskExtensions;

public class NakamaCon
{
	public static NakamaCon Ins { get; private set; } = new NakamaCon();

	public ISocket Socket => _socket;

	//public Action<IMatchmakerMatched> ReceivedMatchmakerMatched { get; internal set; }

	private Client _client;
	private ISession _session;
	private ISocket _socket;

	const string scheme = "http";
	const string host = "localhost";
	const int port = 7350;
	const string serverKey = "defaultkey";

	IMatch _match;

	string deviceId = Guid.NewGuid().ToString();//"AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";

	private NakamaCon()
	{

	}

	public async Task Init()
	{
		if (_client != null)
			return;

		_client = new Client(scheme, host, port, serverKey);
		_socket = Nakama.Socket.From(_client);
		_session = await _client.AuthenticateDeviceAsync(deviceId);

		GD.Print(_session);

		Socket.Connected += Socket_Connected;
		Socket.Closed += Socket_Closed;

		await _socket.ConnectAsync(_session);

		Socket.ReceivedMatchmakerMatched += Socket_ReceivedMatchmakerMatched;
	}

	private async void Socket_Closed()
	{
		GD.Print("Socket Closed");
	}

	private void Socket_Connected()
	{
		GD.Print("Socket Connected");
	}

	private async void Socket_ReceivedMatchmakerMatched(IMatchmakerMatched obj)
	{
		_match = await Socket.JoinMatchAsync(obj);
	}

	public async Task<bool> LeaveCurrentMatch()
	{
		if (_match == null)
			return false;

		await Socket.LeaveMatchAsync(_match);
		_match = null;

		return true;
	}

	public async Task SendMatchState(long opCode, object state)
	{
		try
		{
			var seri = fJSON.Serialize(state);
			GD.Print(seri);
		await Socket.SendMatchStateAsync(_match.Id, opCode, seri);
		}
		catch (Exception ex)
		{
			GD.PrintErr("hat happend", ex);
			throw;
		}
	}

}
