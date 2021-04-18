import React, { useState, useEffect } from 'react';
import { makeStyles } from '@material-ui/core/styles';
import { Paper, ListItem, Button, CircularProgress, TextField } from '@material-ui/core';
import copyImg from '../images/discord-copy.png';

export const Settings = (props) => {
  const classes = useStyles();

  const [discordId, setDiscordId] = useState('');
  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    let isMounted = true;
    setIsLoading(true);
    fetch(`${process.env.REACT_APP_API_URI}/api/user/getSettings`, { credentials: 'include' })
      .then(response => response.json())
      .then(d => {
        isMounted && d.discordId && setDiscordId(d.discordId);
      })
      .catch( err => { })
      .finally(() => isMounted && setIsLoading(false));
    return () => { isMounted = false };
  }, []);

  const updateSettings = () => {
    const userSettings = { steamId: props.userInfo.steamId, discordId };
    fetch(`${process.env.REACT_APP_API_URI}/api/user/updateSettings`, {
      credentials: 'include',  
      headers: {
        'content-type': 'application/json'
      },
      body: JSON.stringify(userSettings),
      method: 'POST'
    })
    .catch( err => { })    
    .finally(() => setIsLoading(false));
  }
  
  return (
    <Paper elevation={3} className={classes.container}>
      <h5>Settings</h5>
      <ListItem>
        <span>RLSF has a Discord Bot that can send you match notifications. Simply join this server <a href='https://discord.gg/YtSDmFf5qd' target="_blank" rel="noopener noreferrer">https://discord.gg/YtSDmFf5qd</a>, and then update your Discord ID below.</span>
      </ListItem>
      <ListItem><img src={copyImg} /></ListItem>
      <ListItem>
        <TextField label="Discord ID#XXXX" value={discordId} onChange={(e) => setDiscordId(e.target.value)}/>
      </ListItem>
      <ListItem>
      {(isLoading || props.userIsLoading) && <CircularProgress /> || 
        <Button onClick={() => updateSettings()} color="primary" variant="outlined">
          Update
        </Button>}
      </ListItem>
    </Paper>
  );
}

const useStyles = makeStyles({
  container:{
    padding: 10
  }
});