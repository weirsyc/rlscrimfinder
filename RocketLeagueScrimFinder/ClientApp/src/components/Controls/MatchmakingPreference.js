import React from 'react';
import { makeStyles } from '@material-ui/core/styles';
import { Select, MenuItem, InputLabel, FormControl } from '@material-ui/core';
export const MatchmakingPreference = props => {
  const classes = useStyles();
  return (
    <FormControl>
      <InputLabel>
        Matchmaking Preference
      </InputLabel>
      <Select value={props.value} onChange={props.onChange} className={classes.matchmakingPreference}>
        <MenuItem value={0}>Close ({(props.mmr - 50)} to {(props.mmr + 50)})</MenuItem>
        <MenuItem value={1}>Medium ({(props.mmr - 100)} to {(props.mmr + 100)})</MenuItem>
        <MenuItem value={2}>Far ({(props.mmr - 200)} to {(props.mmr + 200)})</MenuItem>
        <MenuItem value={3}>Any</MenuItem>
      </Select>
    </FormControl>
  );
}
const useStyles = makeStyles({
  matchmakingPreference: {
    minWidth: 400
  },
});