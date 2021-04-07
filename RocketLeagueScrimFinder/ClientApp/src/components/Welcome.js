import React from 'react';
import { makeStyles } from '@material-ui/core/styles';
import { ListItem, ListItemText, Divider } from '@material-ui/core';
import DoubleArrowIcon from '@material-ui/icons/DoubleArrow';


export const Welcome = () => {
  const classes = useStyles();
  return (
    <>
      <p>Welcome to RL Scrim Finder. The purpose of this site is to help players quickly and easily find scrims at their level, 
        instead of having to post in a dozen Discords only to be paired with a team who is nowhere near your MMR.
        <br/><br />
        To get started, sign-in with your Steam account, then navigate to either the Matchmaking or Schedule page.
        <br/><br />
        Matchmaking: This page should be used when you have a 3 stack ready to play. The system will automatically pair you with an opponent and display lobby info. 
        To avoid getting paired against the same opponent multiple times in a row, whenever
        you leave or deny a match the system will not pair you against the same opponent again for 30 minutes.
        <br/><br />
        Scheduling: This page is used to setup future matches. You can create events, request to join others, and manage match requests. 
        When a request is accepted, you can join and leave the lobby at anytime (feel free to leave messages for your opponent).
        The lobby will remain joinable until after the scheduled date.</p>
        <br />
      <h5>FAQ</h5>
      <ListItem>
        <ListItemText>
          <div className={classes.question}>Is it safe to login? What are you tracking?</div>
          <div className={classes.answer}><DoubleArrowIcon className={classes.arrow}/>The login uses Open ID and so you connect directly through Steam, 
          and is only used to lookup your display name and get your MMR through tracker.gg.</div>
        </ListItemText>
      </ListItem>
      <Divider />
      <ListItem>
        <ListItemText>
          <div className={classes.question}>Can I search for 2s or 1s?</div>
          <div className={classes.answer}><DoubleArrowIcon className={classes.arrow}/>No, this is only for 3s at the moment.</div>
        </ListItemText>
      </ListItem>
      <Divider />
      <ListItem>
        <ListItemText>
          <div className={classes.question}>Is there a LFG feature?</div>
          <div className={classes.answer}><DoubleArrowIcon className={classes.arrow}/>No, find some friends lol.</div>
        </ListItemText>
      </ListItem>
      <Divider />
      <ListItem>
        <ListItemText>
          <div className={classes.question}>I just won some ranked games, and my MMR is not updating</div>
          <div className={classes.answer}><DoubleArrowIcon className={classes.arrow}/>MMR values get cached for an hour. Come back later, and your MMR should be updated.</div>
        </ListItemText>
      </ListItem>
      <Divider />
      <ListItem>
        <ListItemText>
          <div className={classes.question}>Upcoming features:</div>
          <div className={classes.answer}><DoubleArrowIcon className={classes.arrow}/>Scheduling page: Done!</div>
          <div className={classes.answer}><DoubleArrowIcon className={classes.arrow}/>Dark mode</div>
          <div className={classes.answer}><DoubleArrowIcon className={classes.arrow}/>Epic Games login</div>
          <div className={classes.answer}><DoubleArrowIcon className={classes.arrow}/>Scheduling improvements (postponing, prevent double-booking, etc)</div>
        </ListItemText>
      </ListItem>
    </>
  );
}

const useStyles = makeStyles({
  container:{
    padding: 10
  },
  question: {
    
  },
  answer: {
    
  },
  arrow: {
    verticalAlign: 'top'
  }
});