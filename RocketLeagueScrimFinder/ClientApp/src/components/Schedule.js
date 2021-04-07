import React, { useState, useEffect} from 'react';
import { makeStyles } from '@material-ui/core/styles';
import { CircularProgress, Grid, Snackbar } from '@material-ui/core';
import { Alert } from '@material-ui/lab';
import { HubConnectionBuilder } from '@microsoft/signalr';
import { MyUpcomingEvents } from './Schedule/MyUpcomingEvents';
import { AvailableEvents } from './Schedule/AvailableEvents';
import { EventRequests } from './Schedule/EventRequests';
import { LobbyInfo } from './LobbyInfo';
import { Chatbox } from './Chatbox';
import { Redirect } from 'react-router';
import moment from 'moment';


export const Schedule = (props) => {
  const classes = useStyles();
  const [lobbyId, setLobbyId] = useState(Number(localStorage.getItem('lobbyId')) || 0);
  const [lobbyInfo, setLobbyInfo] = useState();
  const [scrimEvents, setScrimEvents] = useState([]);
  const [isLoading, setIsLoading] = useState(false);
  const [alertIsOpen, setAlertIsOpen] = useState();
  const [alertMessage, setAlertMessage] = useState();

  useEffect(() => {
    setIsLoading(true);
    let isMounted = true;
    const hubConnection = new HubConnectionBuilder()
      .withUrl(`${process.env.REACT_APP_API_URI}/apphub`)
      .withAutomaticReconnect()
      .build();

      hubConnection.on("MessageReceived", (message) => {
        isMounted && setScrimEvents(se => {
          const scrimLobby = se.find(s => s.id === message.scrimEventId)
          if (!scrimLobby){
            return se;
          }
          scrimLobby.chatLogs = [...scrimLobby.chatLogs, message]
          return [...se.filter(s => s.id !== message.scrimEventId), scrimLobby]
        });
      });

      hubConnection.on("RequestAdded", (request) => {
        console.log("Request added")
        console.log(request);
        isMounted && eventRequested(request);
      });

      hubConnection.on("RequestAccepted", (e) => {
        console.log("Request accepted")
        console.log(e);
        isMounted && requestAccepted(e);
      });

      hubConnection.on("RequestDeleted", (id) => {
        console.log("Request deleted")
        console.log(id);
        isMounted && requestDeleted(id);
      });

      hubConnection.on("EventAdded", (e) => {
        console.log("Event added")
        console.log(e);
        isMounted && eventAdded(e);
      });

      hubConnection.on("EventDeleted", (id) => {
        console.log("Event deleted")
        console.log(id);
        isMounted && eventDeleted(id);
      });

    hubConnection.onclose(()=> { 
      setAlertMessage("Connection lost. Please refresh this page.");
      setAlertIsOpen(true);
    });
    hubConnection
      .start()
      .then((e) => {
          console.log('Connection started!');
          
          const scrimEventsFetch = fetch(`${process.env.REACT_APP_API_URI}/api/schedule/getScrimEvents`, {credentials: 'include'})
            .then(response => response.json())
            .then(data => {
              isMounted && setScrimEvents(se => [...se.filter(e => data.some(ne => e.id === ne.id)), ...data])
            })
            .catch( err => { });

          Promise.all([scrimEventsFetch]).then(() => isMounted && setIsLoading(false));
      })
      .catch((err) => {
        setAlertMessage("Failed to connect to server. Please try again later.");
        setAlertIsOpen(true);
        console.log(err);
      });
      return () => { isMounted = false };
  }, []);

  useEffect(() => {
    let isMounted = true;
    if (!!lobbyId){
      fetch(`${process.env.REACT_APP_API_URI}/api/schedule/getLobbyInfo`, {
        credentials: 'include',
        headers: {
            'content-type': 'application/json'
        },
        body: JSON.stringify(lobbyId),
        method: 'POST'
      })
      .then(response => response.json())
      .then(lobbyInfo => {
        isMounted && setLobbyInfo(lobbyInfo);
      })
      .catch( err => { });
    }
    return () => { isMounted = false };
  }, [scrimEvents, lobbyId]);

  const joinLobby = (lid) => {
    localStorage.setItem('lobbyId', lid);
    setLobbyId(lid);
  }

  const leaveLobby = () => {
    localStorage.removeItem('lobbyId');
    setLobbyId(0);
  }

  const sendMessage = (lobbyId, newMessage) => {
    fetch(`${process.env.REACT_APP_API_URI}/api/schedule/sendMessage`, {
      credentials: 'include',
      headers: {
          'content-type': 'application/json'
      },
      body: JSON.stringify({ scrimEventId: lobbyId, message: newMessage.data.text || newMessage.data.emoji }),
      method: 'POST'
    })
    .catch( err => { });
    return false;
  }

  const eventDeleted = (id) => {
    setScrimEvents(se => [...se.filter(e => e.id !== id)])
  }

  const eventAdded = (ne) => {
    setScrimEvents(se => [...se.filter(e => e.id !== ne.id), ne])
  }

  const eventRequested = (r) => {
    setScrimEvents(se => {
      const scrimEvent = se.find(e => e.id === r.scrimEventId);
      scrimEvent.scrimRequests = [...scrimEvent.scrimRequests, r];
      return [...se.filter(e => e.id !== r.scrimEventId), scrimEvent];
    });
  }

  const requestAccepted = (ue) => {
    setScrimEvents(se => [...se.filter(e => e.id !== ue.id), ue])
  }

  const requestDeleted = (id) => {
    setScrimEvents(se => {
      const scrimEvent = se.find(e => e.scrimRequests && e.scrimRequests.some(r => r.id === id));
      if (scrimEvent && scrimEvent.scrimRequests){
        scrimEvent.scrimRequests = [...scrimEvent.scrimRequests.filter(r => r.id !== id)];
        return [...se.filter(e => e.id !== scrimEvent.id), scrimEvent];
      }
      return se;
    });
  }

  const lobbyData = scrimEvents.find(d => d.id === lobbyId);

  const matchmakingData = lobbyData && lobbyInfo && { 
    player1SteamId: lobbyData.steamId,
    player1Name: lobbyData.displayName,
    player1Mmr: lobbyData.mmr,
    player2SteamId: lobbyData.opponentSteamId,
    player2Name: lobbyData.opponentDisplayName,
    player2Mmr: lobbyData.opponentMmr,
    player1Left: false,
    player2Left: false,
    serverChoices: (lobbyData.servers && lobbyData.servers.split(',').map(s => Number(s))) || [],
    lobbyName: lobbyInfo.name,
    lobbyPassword: lobbyInfo.password,
    chatLog: (lobbyData.chatLogs || []),
    player1MatchAccepted: true,
    player2MatchAccepted: true
  };
  return (
    (props.userIsLoading === false && !props.userInfo && <Redirect to="/" />) ||
    <div>
      {(isLoading && <CircularProgress />) || <>
        {(lobbyId && lobbyData && props.userInfo && matchmakingData &&
        <>
        <h4 className={classes.pageTitle}>Scheduled Match Lobby - {moment.utc(lobbyData.eventDate).local().format('dddd, MMMM Do, YYYY, h:mma')}</h4>
        <Grid container spacing={4}>
          <Grid item md>
            <LobbyInfo matchmakingData={matchmakingData} isPlayerOne={lobbyData.steamId === props.userInfo.steamId} leaveCurrentLobby={leaveLobby} />
          </Grid>
          <Grid item md>
            <Chatbox 
              chatLog={matchmakingData.chatLog} 
              userInfo={props.userInfo} 
              opponentName={(lobbyData.steamId === props.userInfo.steamId && lobbyData.opponentDisplayName) || lobbyData.displayName } 
              sendMessage={(newMessage) => sendMessage(lobbyId, newMessage)} />
          </Grid>
        </Grid>
        </>)
        ||
        (props.userInfo && <>
        <h4 className={classes.schedulePageTitle}>Schedule</h4>
        <span>Dates and times converted to your timezone</span>
        <Grid className={classes.schedulePage} container spacing={2}>
          <Grid item xs={12}>
            <MyUpcomingEvents userInfo={props.userInfo} scrimEvents={scrimEvents} joinLobby={joinLobby} eventDeleted={eventDeleted} eventAdded={eventAdded}/>
          </Grid>
          <Grid item xs={12}>
            <EventRequests userInfo={props.userInfo} scrimEvents={scrimEvents} requestAccepted={requestAccepted} requestDeleted={requestDeleted} />
          </Grid>
          <Grid item xs={12}>
            <AvailableEvents userInfo={props.userInfo} scrimEvents={scrimEvents} eventRequested={eventRequested} />
          </Grid>
        </Grid>
        </>)
        }
      </>}
      
      <Snackbar open={alertIsOpen} onClose={() => setAlertIsOpen(false)}>
        <Alert onClose={() => setAlertIsOpen(false)} severity="warning">
          {alertMessage}
        </Alert>
      </Snackbar>
    </div>
  );
}

const useStyles = makeStyles({
  container:{
    padding: 10
  },
  schedulePage:{
    '& .scheduleHeader':{
      fontSize: '22px',
      marginTop: '3px'
    }
  },
  schedulePageTitle:{
    display: 'inline-block',
    marginRight: '15px'
  },
  pageTitle: {
    marginBottom: '15px'
  }
  
});