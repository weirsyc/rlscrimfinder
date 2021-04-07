import React, { useState, useEffect } from 'react';
import { makeStyles } from '@material-ui/core/styles';
import { Paper, FormControl, InputLabel, MenuItem, Select, Button, TextField, FormControlLabel, Checkbox, ListItem, ListItemText } from '@material-ui/core';
import { Autocomplete, Alert } from '@material-ui/lab';
import { ServerList } from './ServerList'
import './scrimfinder.css'


export const ScrimFinder = props => {
  const classes = useStyles();
  const [matchmakingPreference, setMatchmakingPreference] = useState(Number(localStorage.getItem('matchmakingPreference')) || 0);
  const [serverSelection, setServerSelection] = useState(JSON.parse(localStorage.getItem('serverSelection')) || []);
  const [collegiateSelected, setCollegiateSelected] = useState(localStorage.getItem('collegiateSelected') === 'true' || false);
  const [errorMessage, setErrorMessage] = useState(false);
  const [playerAccepted, setPlayerAccepted] = useState(false)
  
  useEffect(() => {
    const alreadyAccepted = props.matchmakingData && 
      ((props.matchmakingData.player1SteamId === props.userInfo.steamId && props.matchmakingData.player1MatchAccepted) || 
        (props.matchmakingData.player2SteamId === props.userInfo.steamId &&  props.matchmakingData.player2MatchAccepted) || false)
    setPlayerAccepted(alreadyAccepted);
    fetch(`${process.env.REACT_APP_API_URI}/api/matchmaking/isQueued`, {credentials: 'include'})
      .then(response => response.json())
      .then(userIsQueued => {
        if (userIsQueued){
          props.setIsSearching(true);
        }
      })
      .catch( err => { });
  },[props])

  const updateMatchmakingPreference = (e) => {
    setMatchmakingPreference(e.target.value);
    localStorage.setItem('matchmakingPreference', e.target.value);
    if (props.isSearching){
      fetch(`${process.env.REACT_APP_API_URI}/api/matchmaking/updateMatchmakingPreference`, {
        credentials: 'include',
        headers: {
            'content-type': 'application/json'
        },
        body: JSON.stringify(e.target.value),
        method: 'POST'
      })
      .catch( err => { });
    }
  }
  const updateServerSelection = (event, values) => {
    if (props.isSearching && !values.length){
      return;
    }
    setServerSelection(values.map(v => v.id));
    localStorage.setItem('serverSelection', JSON.stringify(values.map(v => v.id)));
    if (props.isSearching){
      fetch(`${process.env.REACT_APP_API_URI}/api/matchmaking/updateServerSelection`, {
        credentials: 'include',
        headers: {
            'content-type': 'application/json'
        },
        body: JSON.stringify(values.map(v => v.id)),
        method: 'POST'
      })
      .catch( err => { });
    }
  }
  const updateCollegiateChecked = (event) => {
    setCollegiateSelected(event.target.checked);
    localStorage.setItem('collegiateSelected', event.target.checked);
    if (props.isSearching){
      fetch(`${process.env.REACT_APP_API_URI}/api/matchmaking/updateCollegiateChecked`, {
        credentials: 'include',
        headers: {
            'content-type': 'application/json'
        },
        body: JSON.stringify(event.target.checked),
        method: 'POST'
      })
      .catch( err => { });
    }
  }

  const toggleMatchSearch = () => {
    setErrorMessage(undefined);
    if (props.isSearching){
      props.setIsSearching(false);
      fetch(`${process.env.REACT_APP_API_URI}/api/matchmaking/cancelSearch`, {
        credentials: 'include',
        method: 'POST'
      })
      .catch( err => {
        setErrorMessage("Failed to cancel matchmaking.");
        props.setIsSearching(true);
      });
    }
    else{
      if (!serverSelection || !serverSelection.length){
        setErrorMessage('Please select at least one server.')
        return;
      }
      props.setIsSearching(true);
      fetch(`${process.env.REACT_APP_API_URI}/api/matchmaking/startSearch`, {
        credentials: 'include',
        headers: {
            'content-type': 'application/json'
        },
        body: JSON.stringify({ matchmakingPreference, servers: serverSelection, collegiate: collegiateSelected, displayName: props.userInfo.displayName }),
        method: 'POST'
      })
      .catch( err => {
        setErrorMessage("Failed to start matchmaking search.");
        props.setIsSearching(false);
      });
    }
  }

  
  const acceptMatch = () => {
    fetch(`${process.env.REACT_APP_API_URI}/api/matchmaking/acceptMatch`, {
      credentials: 'include',
      method: 'POST'
    })
    .then(() => setPlayerAccepted(true))
    .catch( err => { });
  }
  
  const declineMatch = () => {
    fetch(`${process.env.REACT_APP_API_URI}/api/matchmaking/declineMatch`, {
      credentials: 'include',
      method: 'POST'
    })
    .catch( err => { });
  }


  const opponentName = props.matchmakingData && 
    (props.matchmakingData.player1SteamId === props.userInfo.steamId ? props.matchmakingData.player2Name : props.matchmakingData.player1Name)
  const opponentSteamId = props.matchmakingData && 
    (props.matchmakingData.player1SteamId === props.userInfo.steamId ? props.matchmakingData.player2SteamId : props.matchmakingData.player1SteamId)
  const opponentProfile = 'https://steamcommunity.com/profiles/' + opponentSteamId;
  
  return (
    <Paper elevation={3} className={classes.container}>
      <ListItem>
        <ListItemText>Your MMR: {props.userInfo.mmr}</ListItemText>
      </ListItem>
      <ListItem>
        <FormControl>
          <InputLabel>
            Matchmaking Preference
          </InputLabel>
          <Select value={matchmakingPreference} onChange={updateMatchmakingPreference} className={classes.matchmakingPreference}>
            <MenuItem value={0}>Close ({(props.userInfo.mmr - 50)} to {(props.userInfo.mmr + 50)})</MenuItem>
            <MenuItem value={1}>Medium ({(props.userInfo.mmr - 100)} to {(props.userInfo.mmr + 100)})</MenuItem>
            <MenuItem value={2}>Far ({(props.userInfo.mmr - 200)} to {(props.userInfo.mmr + 200)})</MenuItem>
            <MenuItem value={3}>Any</MenuItem>
          </Select>
        </FormControl>
      </ListItem>
      <ListItem>
        <Autocomplete className={classes.serverList}
          value={[...ServerList.filter(s => serverSelection.some(sid => s.id === sid))]} multiple 
          options={ServerList} getOptionLabel={(option) => option.title}
          onChange={updateServerSelection}
          renderInput={(params) => (
          <TextField
            {...params}
            label="Server(s)"
          />
          )}
        />
        </ListItem>
        <ListItem>
        <FormControlLabel
          control={
            <Checkbox
              onChange={updateCollegiateChecked}
              checked={collegiateSelected}
              color="primary"
            />
          }
          label="Collegiate"
        />
        </ListItem>
        <ListItem>
      <Button variant="outlined" color="primary" onClick={toggleMatchSearch}>
        {props.isSearching ? `Cancel Search` : `Begin Search`}
      </Button>
      </ListItem>
      <ListItem>
        <ListItemText>{errorMessage}</ListItemText>
      </ListItem>
      {!!props.matchmakingData && 
      <ListItem>
        <Alert severity="success">
          <div className="match-found">Match found! Opponent: <a href={opponentProfile} target="_blank" rel="noopener noreferrer">{opponentName}</a></div>
          {(playerAccepted && <span className="waiting-message">Waiting for opponent</span>) || 
          (<Button variant="outlined" className="accept-button" onClick={acceptMatch}>Accept</Button>)}
          <Button variant="outlined" className="decline-button" color="secondary" onClick={declineMatch}>Decline</Button>
        </Alert>
      </ListItem>
      }
    </Paper>
  );
}

const useStyles = makeStyles({
  container:{
    paddingTop: 10
  },
  serverList: {
    minWidth: 400
  },
  matchmakingPreference: {
    minWidth: 400
  },
});