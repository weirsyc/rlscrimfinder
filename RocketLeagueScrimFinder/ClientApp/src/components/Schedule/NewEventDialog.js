import React, { useState } from 'react';
import { makeStyles } from '@material-ui/core/styles';
import { TextField, ListItem, Button, FormControl, FormControlLabel, Checkbox, Dialog, DialogActions, CircularProgress } from '@material-ui/core';
import { ServerSelection } from '../Controls/ServerSelection';
import moment from 'moment';


export const NewEventDialog = props => {
  const classes = useStyles();

  //const [teamName, setTeamName] = useState(localStorage.getItem('teamName') || '');
  const [eventDate, setEventDate] = useState((new Date()).toISOString().substr(0, 10) + "T20:00");
  //const [matchmakingPreference, setMatchmakingPreference] = useState(Number(localStorage.getItem('matchmakingPreference')) || 0);
  const [serverSelection, setServerSelection] = useState(JSON.parse(localStorage.getItem('serverSelection')) || []);
  const [collegiateSelected, setCollegiateSelected] = useState(localStorage.getItem('collegiateSelected') === 'true' || false);
  const [isLoading, setIsLoading] = useState(false);
  const [resultMessage, setResultMessage] = useState();
  
  /*const updateTeamName = (event) => {
    setTeamName(event.target.value);
    localStorage.setItem('teamName', event.target.value);
  }
  const updateMatchmakingPreference = (e) => {
    setMatchmakingPreference(e.target.value);
    localStorage.setItem('matchmakingPreference', e.target.value);
  }*/
  
  const updateServerSelection = (event, values) => {
    setServerSelection(values.map(v => v.id));
    localStorage.setItem('serverSelection', JSON.stringify(values.map(v => v.id)));
  }

  const updateCollegiateChecked = (event) => {
    setCollegiateSelected(event.target.checked);
    localStorage.setItem('collegiateSelected', event.target.checked);
  }

  const submitNewEvent = () => {
    setResultMessage(undefined);
    if (!eventDate || !moment(eventDate)){
      setResultMessage('Please set a valid date & time.');
      return false;
    }
    if (!moment(eventDate).isSameOrAfter(new Date(), 'day')){
      setResultMessage('Cannot select past dates.');
      return false
    }
    if (!serverSelection || !serverSelection.length){
      setResultMessage('Please select at least one server.');
      return false;
    }
    setIsLoading(true);

    fetch(`${process.env.REACT_APP_API_URI}/api/schedule/add`, {
      credentials: 'include',
      headers: {
          'content-type': 'application/json'
      },
      body: JSON.stringify({ /*teamName,*/ eventDate: moment(eventDate).utc(), /*matchmakingPreference,*/ servers: serverSelection.join(','), collegiate: collegiateSelected }),
      method: 'POST'
    })
      .then(response => response.json())
      .then(newEvent => {
        props.onClose(true, newEvent);
      })
      .catch( err => { 
        console.log(err);
        setResultMessage('Failed to create new event.');
      })
      .finally(() => setIsLoading(false));
  }

  return (
    <Dialog open={props.open} maxWidth={'sm'} fullWidth={true}>
      {/*<ListItem>
        <FormControl>
          <TextField label="Team name" value={teamName} onChange={updateTeamName} />
        </FormControl>
      </ListItem>*/}
      <div className={classes.dialog}>
        <ListItem>
          <h5>New Scrim</h5>
        </ListItem>
        <ListItem>
          <FormControl>   
            <TextField label="Date &amp; time" type="datetime-local" value={eventDate} onChange={(e) => {setEventDate(e.target.value)}}
              InputLabelProps={{
                shrink: true
              }}
            />        
          </FormControl>
        </ListItem>
        {/*<ListItem>
          <MatchmakingPreference value={matchmakingPreference} onChange={updateMatchmakingPreference} mmr={props.userInfo && props.userInfo.mmr || 0}/>
        </ListItem>*/}
        <ListItem>
          <ServerSelection value={serverSelection} onChange={updateServerSelection}/>
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
        <DialogActions>
          <Button onClick={() => props.onClose(false)} color="primary">
            Cancel
          </Button>
          {(isLoading && <CircularProgress />) || 
          <Button className="submit-button" onClick={submitNewEvent}>Submit</Button>}
        </DialogActions>
        {resultMessage && 
          <div className={classes.resultMessage}>
            {resultMessage}
          </div>
        }
      </div>
    </Dialog>
  );
}

const useStyles = makeStyles({
  dialog:{
    padding: '30px !important',
  },
  resultMessage: {
    textAlign: 'right'
  }
});