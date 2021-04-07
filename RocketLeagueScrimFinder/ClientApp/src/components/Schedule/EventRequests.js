import React, { useState } from 'react';
import { makeStyles } from '@material-ui/core/styles';
import { Paper, ListItem, Button, TableContainer, Table, TableBody, TableRow, TableCell, CircularProgress } from '@material-ui/core';
import { ToggleButton, ToggleButtonGroup } from '@material-ui/lab';
import moment from 'moment';


export const EventRequests = props => {
  const classes = useStyles();
  const [view, selectView] = useState('left');
  const [isLoading, setIsLoading] = useState(false);

  const acceptRequest = (id) => {
    setIsLoading(true);
    fetch(`${process.env.REACT_APP_API_URI}/api/schedule/acceptRequest`, {
      credentials: 'include',
      headers: {
          'content-type': 'application/json'
      },
      body: JSON.stringify(id),
      method: 'POST'
    })
      .then(response => response.json())
      .then((se) => {
        props.requestAccepted(se);
      })
      .catch( err => { 
        console.log(err);
        //todo: error
      })
      .finally(() => setIsLoading(false));

  }
  const deleteRequest = (id) => {
    setIsLoading(true);
    fetch(`${process.env.REACT_APP_API_URI}/api/schedule/deleteRequest`, {
      credentials: 'include',
      headers: {
          'content-type': 'application/json'
      },
      body: JSON.stringify(id),
      method: 'POST'
    })
      .then(() => {
        props.requestDeleted(id);
      })
      .catch( err => { 
        console.log(err);
        //todo: error
      })
      .finally(() => setIsLoading(false));
  }

  const formatDate = (date) => {
    return moment.utc(date).local().format('ddd, MMMM D, h:mma');
  }

  const requestEvents = props.scrimEvents.filter(se => moment.utc(se.eventDate).local().isSameOrAfter(moment(), 'day') && 
    se.scrimRequests && se.scrimRequests.length && 
    se.steamId === props.userInfo.steamId && 
    !se.opponentSteamId)
    .sort((a,b) => new Date(a.eventDate) - new Date(b.eventDate));
  const sentRequestEvents = props.scrimEvents.filter(se => moment.utc(se.eventDate).local().isSameOrAfter(moment(), 'day') &&
    se.scrimRequests && 
    se.scrimRequests.some(r => r.steamId === props.userInfo.steamId) && 
    !se.opponentSteamId)
    .sort((a,b) => new Date(a.eventDate) - new Date(b.eventDate));
  const emptyMessage = (view === 'left' && 'No incoming scrim requests found.') || 'You have not sent any requests for upcoming scrims.'
  return (
    <Paper elevation={3} className={classes.container}>
      <ListItem>
        <h5 className='scheduleHeader'>Invitations</h5>
        <ToggleButtonGroup
          className={classes.toggleButtonGroup}
          value={view}
          exclusive
          onChange={(e, v) => (v && selectView(v))}
        >
          <ToggleButton value="left" size="small" color="primary" className={classes.toggleButton}>
            Match requests
          </ToggleButton>
          <ToggleButton value="right" size="small" className={classes.toggleButton}>
            Sent requests
          </ToggleButton>
        </ToggleButtonGroup>
      </ListItem>
      <TableContainer className={classes.tableContainer}>
        <Table className={classes.table} size="small">
            <TableBody>
              {(view === 'left' && (
                (requestEvents.length && requestEvents.map(e => (
                  e.scrimRequests && e.scrimRequests.length && e.scrimRequests.map((r, i) => (
                    <TableRow key={i}>
                      <TableCell>{formatDate(e.eventDate)}</TableCell>
                      <TableCell>
                        <a href={('https://steamcommunity.com/profiles/' + (r.steamId) )} target="_blank" rel="noopener noreferrer">
                          <span className={classes.displayName}>{r.displayName}</span></a> ({r.mmr})
                      </TableCell>
                      <TableCell>
                        {(isLoading && <CircularProgress size={25} />) || <>
                        <Button size="small" variant="outlined" className={classes.acceptButton} onClick={() => acceptRequest(r.id)}>Accept</Button>
                        <Button size="small" variant="outlined" color="secondary" onClick={() => deleteRequest(r.id)}>Delete</Button>
                        </>}
                      </TableCell>
                    </TableRow>
                    ))
                    ))) || <TableRow><TableCell><i>{emptyMessage}</i></TableCell></TableRow>
                  )) ||
                ((sentRequestEvents.length && sentRequestEvents.map((e, i) => {
                  const sr = e.scrimRequests.find(r => r.steamId === props.userInfo.steamId)
                  return (
                  <TableRow key={i}>
                    <TableCell>{formatDate(e.eventDate)}</TableCell>
                    <TableCell>
                      <a href={('https://steamcommunity.com/profiles/' + (e.steamId) )} target="_blank" rel="noopener noreferrer">{e.displayName}</a> ({e.mmr})
                    </TableCell>
                    <TableCell>
                      <Button size="small" variant="outlined" color="secondary" onClick={() => deleteRequest(sr.id)}>Delete</Button>
                    </TableCell>
                  </TableRow>
                  )})) || <TableRow><TableCell><i>{emptyMessage}</i></TableCell></TableRow>
              )
              }
            </TableBody>
        </Table>
      </TableContainer>
    </Paper>
  );
}

const useStyles = makeStyles({
  toggleButtonGroup:{
    marginLeft: 10
  },
  emptyTableMessage:{

  },
  toggleButton: {
    color: '#3f51b5 !important',
    border: '1px solid rgba(63, 81, 181, 0.5) !important'
  },
  table:{
    width: 'auto',
    '& td':{
      borderBottom: 'none'
    },
    marginBottom: '10px'
  },
  tableContainer:{
    maxHeight: '200px',
    minHeight: '52px'
  },
  acceptButton:{
    marginRight: '10px'
  },
  //TODO: style better
  displayName:{
    /*display: 'inline-block',
    verticalAlign: 'middle',
    maxWidth: '120px',
    whiteSpace: 'nowrap',
    overflow: 'hidden',
    textOverflow: 'ellipsis'*/
  }
});