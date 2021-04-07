import React from 'react';
import { makeStyles } from '@material-ui/core/styles';
import { Paper, Button, ListItem, ListItemText } from '@material-ui/core';
import { ServerList } from './ServerList';
import { Alert } from '@material-ui/lab';

export const LobbyInfo = props => {
  const classes = useStyles();
  const opponentSteamId = props.isPlayerOne ? props.matchmakingData.player2SteamId : props.matchmakingData.player1SteamId;
  const opponentProfile = 'https://steamcommunity.com/profiles/' + opponentSteamId;
  const opponentName = props.isPlayerOne ? props.matchmakingData.player2Name : props.matchmakingData.player1Name;
  const opponentMmr = props.isPlayerOne ? props.matchmakingData.player2Mmr : props.matchmakingData.player1Mmr;
  const opponentLeft = props.isPlayerOne ? props.matchmakingData.player2Left : props.matchmakingData.player1Left;
  const serverChoices = props.matchmakingData.serverChoices.map(sid => ServerList.find(s => s.id === sid).title).join(' or ');
  return (
    <Paper elevation={3} className={classes.container}>
      <ListItem>
        <ListItemText>Opponent: <a href={opponentProfile} target="_blank" rel="noopener noreferrer">{opponentName}</a> ({opponentMmr})</ListItemText>
      </ListItem>
      <ListItem>
        <ListItemText><b>{props.isPlayerOne ? `Create` : `Join`}</b> the game lobby</ListItemText>
      </ListItem>
      <ListItem>
        <ListItemText>Server: {serverChoices}</ListItemText>
      </ListItem>
      <ListItem>
        <ListItemText>Lobby Name: {props.matchmakingData.lobbyName}</ListItemText>
      </ListItem>
      <ListItem>
        <ListItemText>Password: {props.matchmakingData.lobbyPassword}</ListItemText>
      </ListItem>
      <ListItem style={{flexGrow: 1}}></ListItem>
      <ListItem>
        <Button variant="outlined" color="primary" onClick={props.leaveCurrentLobby}>
          Leave this lobby
        </Button>
      </ListItem>
      <ListItem>
        {opponentLeft && (
          <Alert severity="warning">
            Opponent left the lobby
          </Alert>
        )}
      </ListItem>
    </Paper>
  );
}

const useStyles = makeStyles({
  container:{
    paddingTop: 10,
    paddingBottom: 10,
    height: '100%',
    display: 'flex',
    flexDirection: 'column'
  }
});