import React, { useState } from 'react';
import { makeStyles } from '@material-ui/core/styles';
import { Paper, ListItem, Button, TableContainer, Table, TableBody, TableRow, TableCell, CircularProgress } from '@material-ui/core';
import { NewEventDialog } from './NewEventDialog';
import moment from 'moment';


export const MyUpcomingEvents = props => {
  const classes = useStyles();
  const [newEventDialogOpen, setNewEventDialogOpen] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  const deleteEvent = (e) => {
    setIsLoading(true);
    fetch(`${process.env.REACT_APP_API_URI}/api/schedule/deleteEvent`, {
      credentials: 'include',
      headers: {
          'content-type': 'application/json'
      },
      body: JSON.stringify(e.id),
      method: 'POST'
    })
      .then(() => {
        props.eventDeleted(e.id);
      })
      .catch( err => { 
        console.log(err);
      })
      .finally(() => setIsLoading(false));
  }

  const closeNewEventDialog = (success, newEvent) => {
    if (success){
      props.eventAdded(newEvent);
    }
    setNewEventDialogOpen(false);
  }
  const formatDate = (date) => {
    return moment.utc(date).local().format('ddd, MMMM D, h:mma');
  }

  const myScrimEvents = props.scrimEvents
    .filter(e => moment.utc(e.eventDate).local().isSameOrAfter(moment(), 'day') && 
      (e.steamId === props.userInfo.steamId || e.opponentSteamId === props.userInfo.steamId))
    .sort((a,b) => new Date(a.eventDate) - new Date(b.eventDate));
  return (
    <Paper elevation={3} className={classes.container}>
      <ListItem>
        <h5 className='scheduleHeader'>My Upcoming Scrims</h5>
        <Button className={classes.createButton} variant="outlined" color="primary" onClick={() => setNewEventDialogOpen(true)}>Create</Button>
      </ListItem>
      <TableContainer className={classes.tableContainer}>
        <Table className={classes.table} size="small">
            <TableBody>
            {(myScrimEvents && myScrimEvents.length && myScrimEvents.map((e, i) => (
              <TableRow key={i}>
                <TableCell><Button disabled={!e.opponentSteamId} variant="outlined" color="primary" onClick={() => props.joinLobby(e.id)}>Join lobby</Button></TableCell>
                <TableCell>{formatDate(e.eventDate)}</TableCell>
                <TableCell>
                  {(e.opponentSteamId &&
                  <><a href={('https://steamcommunity.com/profiles/' + ((e.steamId === props.userInfo.steamId && props.userInfo.opponentSteamId) || props.userInfo.steamId))} 
                      target="_blank" rel="noopener noreferrer">{((e.steamId === props.userInfo.steamId && e.opponentDisplayName) || e.displayName)}
                    </a> ({((e.steamId === props.userInfo.steamId && e.opponentMmr) || e.mmr)})</>)
                  || <i>No opponent yet</i>
                  }
                </TableCell>
                {!e.opponentSteamId &&
                <TableCell>
                  {(isLoading && <CircularProgress size={25}/>) || 
                  <Button size="small" variant="outlined" color="secondary" onClick={() => deleteEvent(e)}>Delete</Button>}
                </TableCell>}
              </TableRow>
            ))) || <TableRow><TableCell><i>No upcoming scrims found.</i></TableCell></TableRow>}
            </TableBody>
        </Table>
      </TableContainer>
      <NewEventDialog open={newEventDialogOpen} onClose={closeNewEventDialog}/>
    </Paper>
  );
}

const useStyles = makeStyles({
  createButton: {
    marginLeft: '10px'
  },
  table:{
    width: 'auto',
    '& td':{
      borderBottom: 'none'
    },
    marginBottom: '10px'
  },
  tableContainer: {
    maxHeight: '300px',
    minHeight: '92px'
  }
});