using UnityEngine;
using Unity.Services.Core;


using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class TestLobby : MonoBehaviour
{

    public static TestLobby Instance { get; private set; }

    public static bool IsHost { get; private set; }
    public static string RelayJoinCode { get; private set; }

    public Lobby hostLobby;
    public Lobby joinedLobby;
    public string relayCode;
    private float heartbeatTimer;
    private float lobbyUpdateTimer;
    private string playerName;

    public bool gameStarted;
    public GameObject LobbyUI;

    [SerializeField] private Button createLobbyButton;
    [SerializeField] private TMP_Text lobbyCode;
    [SerializeField] private Button joinLobbyButton;
    [SerializeField] private Button joinLobbyCodeButton;
    [SerializeField] private TMP_InputField enterLobbyCode;
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject LobbyCodeUI;
    [SerializeField] private GameObject joinLobbyUI;

    private void Awake()
    {
        Instance = this;
        joinLobbyCodeButton.onClick.AddListener(openjoinLobbyUI);
        createLobbyButton.onClick.AddListener(CreateLobby);
        joinLobbyButton.onClick.AddListener(handleJoinLobby);
    }

    private async void Start()
    {
       await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };

       //await AuthenticationService.Instance.SignInAnonymouslyAsync();

       playerName = "Lemonade" + UnityEngine.Random.Range(10, 99);
       Debug.Log(playerName);
    }

    private void Update()
    {
        HandleLobbyHeartbeat();
        HandleLobbyPollForUpdates();

        

        if(joinedLobby != null)
        {
            //Debug.Log(joinedLobby.Players.Count);
            if (joinedLobby.Players.Count == 2 && !gameStarted)
            {
                StartGame();
            }
        }
    }

    public bool isLobbyHost()
    {
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    private async void HandleLobbyHeartbeat()
    {
        if (hostLobby != null)
        {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer < 0)
            {
                float heartbeatTimerMax = 15;
                heartbeatTimer = heartbeatTimerMax;

                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
            }
        }
    }

    private async void HandleLobbyPollForUpdates()
    {
        if (joinedLobby != null)
        {
            lobbyUpdateTimer -= Time.deltaTime;
            if (lobbyUpdateTimer < 0)
            {
                float lobbyUpdateTimerMax = 1.1f;
                lobbyUpdateTimer = lobbyUpdateTimerMax;

                Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
                joinedLobby = lobby;

                //Debug.Log(joinedLobby.Data["StartGame_RelayCode"].Value);

                if (joinedLobby.Data["StartGame_RelayCode"].Value != "0")
                {
                    Debug.Log("Trying to join relay");
                    if (!isLobbyHost())
                    {
                        TestRelay.Instance.JoinRelay(joinedLobby.Data["StartGame_RelayCode"].Value);
                        Debug.Log("Relay connection success");
                        cam.gameObject.SetActive(false);
                        LobbyUI.gameObject.SetActive(false);
                    }
                    joinedLobby = null;
                }
            }
        }
    }

    private async void CreateLobby()
    {
        LobbyCodeUI.SetActive(true);
        try
        {
            string lobbyName = "MyLobby";
            int maxPlayers = 2;

            CreateLobbyOptions options = new CreateLobbyOptions
            {
                IsPrivate = true,
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject>
                {
                    {"StartGame_RelayCode", new DataObject(DataObject.VisibilityOptions.Member, "0")}
                //    {"GameMode", new DataObject(DataObject.VisibilityOptions.Public, "Classic") }
                }
            };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);

            //relayCode = await TestRelay.Instance.CreateRelay();
            //Debug.Log("Relay Code: " + relayCode);

            lobbyCode.text = "Lobby Code: " + lobby.LobbyCode;

            IsHost = true;
            hostLobby = lobby;
            joinedLobby = hostLobby;

            Debug.Log("Created Lobby! " + lobby.Name + " " + lobby.MaxPlayers + " " + lobby.Id + " " + lobby.LobbyCode);
            PrintPlayers(lobby);
        }
        catch (LobbyServiceException ex)
        {
            Debug.Log(ex);
        }
    }

    private async void ListLobbies()
    {
        try
        {
            QueryResponse qr = await LobbyService.Instance.QueryLobbiesAsync();

            Debug.Log("Lobbies Found: " + qr.Results.Count);
            foreach (Lobby lobby in qr.Results)
            {
                Debug.Log(lobby.Name + ": " + lobby.MaxPlayers);
            }
        }
        catch (LobbyServiceException ex)
        {
            Debug.Log(ex);
        }
    }

    public async void JoinLobbyByCode(string lobbyCode)
    {
        try
        {
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions
            {
                Player = GetPlayer(),
            };
            Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);

            joinedLobby = lobby;

            Debug.Log("Joined Lobby with code: " + lobbyCode);
            PrintPlayers(lobby);
            
        }
        catch (LobbyServiceException ex)
        {
            Debug.Log(ex);
        }
    }

    private void handleJoinLobby()
    {
        if (enterLobbyCode != null)
        {
            JoinLobbyByCode(enterLobbyCode.text);
        }
    }

    private void openjoinLobbyUI()
    {
        joinLobbyUI.SetActive(!joinLobbyUI.activeInHierarchy);
    }

    private Player GetPlayer()
    {
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>
            {
                {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName)}
            }
        };
    }

    private void PrintPlayers()
    {
        PrintPlayers(joinedLobby);
    }

    private void PrintPlayers(Lobby lobby)
    {
        Debug.Log("Players in Lobby " + lobby.Name);
        foreach (Player player in lobby.Players)
        {
            Debug.Log(player.Id + " " + player.Data["PlayerName"].Value);
        }
    }

    private async void UpdatePlayerName(string newPlayerName)
    {
        try
        {
            playerName = newPlayerName;
            await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions{
                Data = new Dictionary<string, PlayerDataObject>
                {
                    {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName)}
                }
            });
        } 
        catch (LobbyServiceException ex) {
            Debug.Log(ex);
        }
    }

    private async void LeaveLobby()
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
        }
        catch (LobbyServiceException ex)
        {
            Debug.Log(ex);
        }
    }

    private async void KickPlayer()
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, joinedLobby.Players[1].Id);
        }
        catch (LobbyServiceException ex)
        {
            Debug.Log(ex);
        }
    }

    private async void DeleteLobby()
    {
        try
        {
            await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
        }
        catch (LobbyServiceException ex)
        {
            Debug.Log(ex);
        }
    }

    public async void StartGame()
    {
        gameStarted = true;

        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        

        if (isLobbyHost())
        {
            try
            {
                Debug.Log("StartGame");
                string relayCode = await TestRelay.Instance.CreateRelay();

                Lobby lobby = await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                {
                    {"StartGame_RelayCode", new DataObject(DataObject.VisibilityOptions.Member, relayCode) }
                }
                });
                joinedLobby = lobby;
            }
            catch (LobbyServiceException ex)
            {
                Debug.Log(ex);
            }
        }
        cam.gameObject.SetActive(false);
        LobbyUI.gameObject.SetActive(false);
    }
}
