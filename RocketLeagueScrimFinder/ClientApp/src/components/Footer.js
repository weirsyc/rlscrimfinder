import React from 'react';
import { makeStyles } from '@material-ui/core/styles';


export const Footer = () => {
  const classes = useStyles();
  return (
    <div className={classes.footer}>
        Created by <a href="https://steamcommunity.com/id/captaincurtle/" target="_blank" rel="noopener noreferrer">Captain Curtle</a>
    </div>
  );
}

const useStyles = makeStyles({
  footer:{
    fontSize: '60%',
    marginTop: 10,
    marginBottom: 50
  }
});



