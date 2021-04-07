import React, { Component } from 'react';
import { Collapse, Container, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Button } from '@material-ui/core';
import { Link } from 'react-router-dom';
import './NavMenu.css';

export class NavMenu extends Component {
  static displayName = NavMenu.name;

  constructor (props) {
    super(props);

    this.toggleNavbar = this.toggleNavbar.bind(this);
    this.state = {
      collapsed: true
    };
  }

  toggleNavbar () {
    this.setState({
      collapsed: !this.state.collapsed
    });
  }

  render () {
    const pathname = this.props && this.props.location && this.props.location.pathname.toLowerCase();
    return (
      <header>
        <Navbar className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3" light>
          <Container>
            <NavbarBrand tag={Link} to="/">Rocket League Scrim Finder</NavbarBrand>
            <NavbarToggler onClick={this.toggleNavbar} className="mr-2" />
            <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={!this.state.collapsed} navbar>
              <ul className="navbar-nav flex-grow">
                <NavItem>
                  <NavLink tag={Link} className="text-dark" to="/" active={pathname === '/'}>Home</NavLink>
                </NavItem>
                {this.props.userInfo && <>
                <NavItem>
                  <NavLink tag={Link} className="text-dark" to="/Matchmaking" active={pathname === '/matchmaking'}>Matchmaking</NavLink>
                </NavItem>
                <NavItem>
                  <NavLink tag={Link} className="text-dark" to="/Schedule" active={pathname === '/schedule'}>Schedule</NavLink>
                </NavItem>
                </>}
                <NavItem>
                  <NavLink tag={Link} className="text-dark" to="/Report" active={pathname === '/report'}>Report bugs</NavLink>
                </NavItem>
                <NavItem>
                  <NavLink tag={Link} className="text-dark" to="/Donate" active={pathname === '/donate'}>Donate</NavLink>
                </NavItem>
                {this.props.userInfo &&
                <NavItem className="signout-button">
                  <Button variant="outlined" color="primary" href={`${process.env.REACT_APP_API_URI}/api/authentication/signout`}>
                    Sign out
                  </Button>
                </NavItem>
                }
              </ul>
            </Collapse>
          </Container>
        </Navbar>
      </header>
    );
  }
}
