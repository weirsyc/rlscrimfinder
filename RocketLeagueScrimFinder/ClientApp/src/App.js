import React, { useState, useEffect } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { Matchmaking } from './components/Matchmaking';
import { Schedule } from './components/Schedule';
import { Report } from './components/Report';
import { Donate } from './components/Donate';


import './custom.css'
import 'fontsource-roboto';

export const App = () => {
  const [userInfo, setUserInfo] = useState();
  const [userIsLoading, setUserIsLoading] = useState();
  useEffect(() => {
    let isMounted = true;
    setUserIsLoading(true);
    fetch(`${process.env.REACT_APP_API_URI}/api/user/userinfo`, {credentials: 'include'})
      .then(response => response.json())
      .then(userInfo => {
        isMounted && setUserInfo(userInfo);
      })
      .catch( err => { })
      .finally(() => setUserIsLoading(false));
      
    return () => { isMounted = false };
  }, []);

  return (
    <Layout userInfo={userInfo}>
      <Route exact path='/' component={() => <Home userInfo={userInfo} userIsLoading={userIsLoading}/>} />
      <Route exact path='/Matchmaking' component={() => <Matchmaking userInfo={userInfo} userIsLoading={userIsLoading}/>} />
      <Route exact path='/Schedule' component={() => <Schedule userInfo={userInfo} userIsLoading={userIsLoading}/>} />
      <Route exact path='/Report' component={Report} />
      <Route exact path='/Donate' component={Donate} />
    </Layout>
  );
}

export default App;