import React from 'react';
import { makeStyles } from '@material-ui/core/styles';
import { Paper } from '@material-ui/core';


export const Report = () => {
  const classes = useStyles();
  return (
    <Paper elevation={3} className={classes.container}>
      <h5>Report bugs/errors</h5>
      <p></p>
      <p>If you experience issues with the system, please send an email to:</p>
      <p>
        <a href="mailto:incoming+captaincurtle1-rocketleaguescrimfinder-25003579-issue-@incoming.gitlab.com">incoming+captaincurtle1-rocketleaguescrimfinder-25003579-issue-@incoming.gitlab.com</a>
      </p>
    </Paper>
  );
}

const useStyles = makeStyles({
  container:{
    padding: 10
  }
});