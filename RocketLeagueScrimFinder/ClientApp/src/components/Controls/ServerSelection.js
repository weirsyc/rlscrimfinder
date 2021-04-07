import React from 'react';
import { makeStyles } from '@material-ui/core/styles';
import { Autocomplete } from '@material-ui/lab';
import { TextField } from '@material-ui/core';
import { ServerList } from '../ServerList'
export const ServerSelection = props => {
  const classes = useStyles();
  return (
    <Autocomplete className={classes.serverList}
          value={[...ServerList.filter(s => props.value.some(sid => s.id === sid))]} multiple 
          options={ServerList} getOptionLabel={(option) => option.title}
          onChange={props.onChange}
          renderInput={(params) => (
          <TextField
            {...params}
            label="Server(s)"
          />
          )}
        />
  );
}
const useStyles = makeStyles({
  serverList: {
    minWidth: 400
  }
});