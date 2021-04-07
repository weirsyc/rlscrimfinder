import React from 'react';
import { CircularProgress, Paper, Button } from '@material-ui/core';
import { makeStyles } from '@material-ui/core/styles';
import { Link } from 'react-router-dom';
import { Welcome } from './Welcome';

export const Home = (props) => {
  const classes = useStyles();

  return (
    <Paper elevation={3} className={classes.container}>
      {(props.userIsLoading && <CircularProgress />) ||
        <div>
          {(props.userInfo && 
          <div className={classes.headerContainer}>
            <h5>Logged in as: {props.userInfo.displayName} ({props.userInfo.mmr})</h5>
            <Link className={classes.matchmakingButton} to="/Matchmaking"><Button variant="outlined" color="primary">Matchmaking</Button></Link>
            <Link to="/Schedule"><Button variant="outlined" color="primary">Schedule</Button></Link>
          </div>) || 
          <form action={`${process.env.REACT_APP_API_URI}/api/authentication/signin`} method="post">
            <input type="image" alt="Sign-in with Steam" src="https://community.cloudflare.steamstatic.com/public/images/signinthroughsteam/sits_02.png" />
          </form>
          }
        </div>
      }
      <Welcome />
    </Paper>
  );
}
const useStyles = makeStyles({
  container: {
    padding: '10px'
  },
  matchmakingButton:{
    marginRight: '10px'
  },
  headerContainer:{
    marginBottom: '10px'
  }
});