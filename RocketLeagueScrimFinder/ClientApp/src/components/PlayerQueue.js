import React from 'react';
import { makeStyles } from '@material-ui/core/styles';
import { TableContainer, Table, TableHead, TableBody, TableRow, TableCell, Paper, ListItem } from '@material-ui/core';
import { ServerList } from './ServerList';

export const PlayerQueue = props => {
  const classes = useStyles();
  const getServerNames = (selectedServers) => selectedServers.map(sid => ServerList.find(s => s.id === sid).title).join(', ')
  return (
    <Paper elevation={3} className={classes.container}>
      <ListItem>
        <h5 className={classes.title}>Player Queue</h5>
      </ListItem>
      <TableContainer>
        <Table>
            <TableHead>
              <TableRow>
                <TableCell>Steam Name</TableCell>
                <TableCell>MMR</TableCell>
                <TableCell>Servers</TableCell>
                <TableCell>Collegiate</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
            {props.players.map((p, index) => (
              <TableRow key={index}>
                <TableCell><a href={('https://steamcommunity.com/profiles/' + p.steamId)} target="_blank" rel="noopener noreferrer">{p.displayName}</a></TableCell>
                <TableCell>{p.mmr}</TableCell>
                <TableCell>{getServerNames(p.servers)}</TableCell>
                <TableCell>{p.collegiate ? 'Yes' : 'No'}</TableCell>
              </TableRow>
            ))}
            </TableBody>
        </Table>
      </TableContainer>
    </Paper>
  );
}

const useStyles = makeStyles({
  container: {
    maxHeight: 440,
  },
  title:{
    paddingTop: 10,
    paddingBottom: 10
  }
});