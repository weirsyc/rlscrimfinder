import React from 'react';
import { makeStyles } from '@material-ui/core/styles';
import { Paper, ListItem } from '@material-ui/core';


export const Donate = () => {
  const classes = useStyles();
  return (
    <Paper elevation={3} className={classes.container}>
      <ListItem>Paypal</ListItem>
      <a href={"https://www.paypal.com/donate?business=WY7NQR4EDZ9QU&currency_code=USD"}>
        <input type="image" alt="Paypal" src={"https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif"}/>
      </a>
    </Paper>
  );
}

const useStyles = makeStyles({
  container:{
    padding: 10
  }
});


