import React, { useState, useEffect } from 'react';
import { CircularProgress, Snackbar, Grid } from '@material-ui/core';
import { Alert } from '@material-ui/lab';
import { LobbyInfo } from './LobbyInfo';
import { ScrimFinder } from './ScrimFinder';
import { Chatbox } from './Chatbox';
import { PlayerQueue } from './PlayerQueue';
import { HubConnectionBuilder } from '@microsoft/signalr';
import { Redirect } from 'react-router';

export const Matchmaking = (props) => {
  const [queuedPlayers, setQueuedPlayers] = useState([]);
  const [isLoading, setIsLoading] = useState(false);
  const [matchmakingData, setMatchmakingData] = useState();
  const [alertIsOpen, setAlertIsOpen] = useState();
  const [alertMessage, setAlertMessage] = useState();
  const [isSearching, setIsSearching] = useState(false);
  const [chatLog, setChatLog] = useState([]);

  useEffect(() => {
    setIsLoading(true);
    let isMounted = true;

    const hubConnection = new HubConnectionBuilder()
      .withUrl(`${process.env.REACT_APP_API_URI}/apphub`)
      .withAutomaticReconnect()
      .build();
      
    hubConnection.on("MatchFound", (data) => {
      isMounted && setMatchmakingData(data);
      isMounted && setIsSearching(false);
    });
    hubConnection.on("EnterLobby", () => {
      isMounted && setMatchmakingData(mmData => ({...mmData, player1MatchAccepted: true, player2MatchAccepted: true}))
    });
    hubConnection.on("DeclinedLobby", () => {
      isMounted && setAlertMessage("Match was declined.");
      isMounted && setAlertIsOpen(true);
      isMounted && setMatchmakingData(undefined);
      isMounted && setIsSearching(true);
    });
    hubConnection.on("OpponentLeftLobby", (opponentSteamId) => {
      isMounted && setMatchmakingData(mmData => ({...mmData, player1Left: opponentSteamId === mmData.player1SteamId, player2Left: opponentSteamId === mmData.player2SteamId}))
    });

    hubConnection.on("PlayerListUpdate", (listChange) => {
      isMounted && setQueuedPlayers(q => [...q.filter(p => p.steamId !== listChange.steamId), listChange]);
    });
    hubConnection.on("PlayerListRemoval", (listChange) => {
      isMounted && setQueuedPlayers(q => [...q.filter(p => p.steamId !== listChange.steamId)]);
    });
    hubConnection.on("MessageReceived", (message) => {
      isMounted && setChatLog(c => [...c, message]);
    });

    hubConnection.onclose(()=> { 
      isMounted && setAlertMessage("Connection lost. Please refresh this page.");
      isMounted && setAlertIsOpen(true);
    });

    hubConnection
      .start()
      .then((e) => {
          console.log('Connection started!');
          const existingLobbyFetch = fetch(`${process.env.REACT_APP_API_URI}/api/matchmaking/existingLobby`, {credentials: 'include'})
            .then(response => response.json())
            .then(mmData => {
              isMounted && setMatchmakingData(mmData);
              isMounted && setChatLog(mmData.chatLog || [])
            })
            .catch( err => { });
          Promise.all([existingLobbyFetch]).then(() => isMounted && setIsLoading(false));
    })
    .catch((err) => {
      isMounted && setAlertMessage("Failed to connect to server. Please try again later.");
      isMounted && setAlertIsOpen(true);
      console.log(err);
    });
    
    isMounted && refreshPlayerQueue();
    
    return () => { isMounted = false };
  }, []);

  const refreshPlayerQueue = () => {
    return fetch(`${process.env.REACT_APP_API_URI}/api/matchmaking/queuedPlayers`, {credentials: 'include'})
      .then(response => response.json())
      .then(playerInfo => {
        setQueuedPlayers(playerInfo);
      })
      .catch( err => { });
  }
  
  const leaveLobby = () => {
    setIsLoading(true);
    setChatLog([]);
    fetch(`${process.env.REACT_APP_API_URI}/api/matchmaking/leaveLobby`, {
      credentials: 'include',
      headers: {
          'content-type': 'application/json'
      },
      method: 'POST'
    })
    .then(() => {
      setMatchmakingData(undefined);
      refreshPlayerQueue();
    })
    .catch( err => { })
    .finally(() => setIsLoading(false))
  }

  const sendMessage = (newMessage) => {
    fetch(`${process.env.REACT_APP_API_URI}/api/chat/sendMessage`, {
      credentials: 'include',
      headers: {
          'content-type': 'application/json'
      },
      body: JSON.stringify(newMessage.data.text || newMessage.data.emoji),
      method: 'POST'
    })
    .catch( err => { });
    return false;
  };

  const sortedQueuedPlayers = queuedPlayers.sort((a, b) => a.mmr - b.mmr);

  return (
    (props.userIsLoading === false && !props.userInfo && <Redirect to="/" />) ||
    <div>
      {(isLoading && <CircularProgress />) ||
        (matchmakingData && matchmakingData.player1MatchAccepted && matchmakingData.player2MatchAccepted && 
        <Grid container spacing={4}>
          <Grid item md>
            <LobbyInfo matchmakingData={matchmakingData} isPlayerOne={matchmakingData.player1SteamId === props.userInfo.steamId} leaveCurrentLobby={leaveLobby} />
          </Grid>
          <Grid item md>
            <Chatbox 
              chatLog={chatLog} 
              userInfo={props.userInfo} 
              opponentName={matchmakingData.player1SteamId === props.userInfo.steamId ? matchmakingData.player2Name : matchmakingData.player1Name} 
              sendMessage={sendMessage} />
          </Grid>
        </Grid>)
        ||
        (props.userInfo &&
        <Grid container spacing={3}>
          <Grid item md>
            <ScrimFinder userInfo={props.userInfo} matchmakingData={matchmakingData} isSearching={isSearching} setIsSearching={setIsSearching}/>
          </Grid>
          <Grid item md>
            <PlayerQueue players={sortedQueuedPlayers}/>
          </Grid>
        </Grid>)
      }

      <Snackbar open={alertIsOpen} onClose={() => setAlertIsOpen(false)}>
        <Alert onClose={() => setAlertIsOpen(false)} severity="warning">
          {alertMessage}
        </Alert>
      </Snackbar>
    </div>
  );
}