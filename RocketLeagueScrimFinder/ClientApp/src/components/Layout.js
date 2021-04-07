import React from 'react';
import { Container } from 'reactstrap';
import { NavMenu } from './NavMenu';
import { Footer } from './Footer';
import { useLocation } from 'react-router-dom';

export const Layout = (props) => {
  let location = useLocation();

  return (
    <div>
      <NavMenu location={location} userInfo={props.userInfo}/>
      <Container>
        {props.children}
      </Container>
      <Container>
        <Footer />
      </Container>
    </div>
  );
}
