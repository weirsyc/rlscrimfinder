import React, { useState, useEffect, useRef } from 'react';
import { makeStyles } from '@material-ui/core/styles';
import { Paper, Grid, ListItem, Button, CircularProgress } from '@material-ui/core';
import FullCalendar from '@fullcalendar/react'
import dayGridPlugin from '@fullcalendar/daygrid'
import listPlugin from '@fullcalendar/list';
import interactionPlugin from '@fullcalendar/interaction';
import { ServerList } from '../ServerList';
import moment from 'moment';

export const AvailableEvents = props => {
  const classes = useStyles();
  const [selectedDate, setSelectedDate] = useState();
  const [isLoading, setIsLoading] = useState(false);
  const calendarComponentRef = useRef(null);
  const listComponentRef = useRef(null);
  const availableEvents = props.scrimEvents.filter(se => moment.utc(se.eventDate).local().isSameOrAfter(moment(), 'day') && 
    !se.opponentSteamId && se.steamId !== props.userInfo.steamId)

  useEffect(() => {
    if (selectedDate){
      calendarComponentRef.current.getApi().gotoDate(selectedDate);
      listComponentRef.current.getApi().gotoDate(selectedDate);
    }
  }, [selectedDate]);

  useEffect(() => {
    if (availableEvents && availableEvents.length){
      //Add or update
      availableEvents.map(e => {
        const event = {
          start: moment.utc(e.eventDate).local().toDate(),
          title: `${e.displayName}\n(${e.mmr})`,
          extendedProps: { data: e },
          id: e.id
        }
        let existingCalendarEvent = calendarComponentRef.current.getApi().getEventById(e.id);
        let existingListEvent = listComponentRef.current.getApi().getEventById(e.id);
        if (existingCalendarEvent){
          existingCalendarEvent.remove();
        }
        if (existingListEvent){
          existingListEvent.remove();
        }
        calendarComponentRef.current.getApi().addEvent(event);
        listComponentRef.current.getApi().addEvent(event);
        return true;
      });
    }
    //Delete
    const allEvents = calendarComponentRef.current.getApi().getEvents();
    allEvents && allEvents.map(e => {
      if (!availableEvents || !availableEvents.length || !availableEvents.some(ae => ae.id === Number(e.id))){
        let existingCalendarEvent = calendarComponentRef.current.getApi().getEventById(e.id);
        let existingListEvent = listComponentRef.current.getApi().getEventById(e.id);
        if (existingCalendarEvent){
          existingCalendarEvent.remove();
        }
        if (existingListEvent){
          existingListEvent.remove();
        }
      }
      return true;
    });
  }, [availableEvents]);

  const requestEvent = (id) => {
    setIsLoading(true);
    fetch(`${process.env.REACT_APP_API_URI}/api/schedule/requestEvent`, {
      credentials: 'include',  
      headers: {
        'content-type': 'application/json'
      },
      body: JSON.stringify(id),
      method: 'POST'
    })
    .then(response => response.json())
    .then(r => {
      props.eventRequested(r);
    })
    .catch( err => { })    
    .finally(() => setIsLoading(false));
  }

  const onEventClick = (info) => {
    setSelectedDate(info.event.start);
    updateHighlightedElement(info.el);
  }
  const onDateClick = (info) => {
    setSelectedDate(info.dateStr);
    updateHighlightedElement(info.dayEl);
  }

  const updateHighlightedElement = (target) => {
    document.querySelectorAll('.fc-day-today').forEach((e) => e.classList.remove('fc-day-today'));
    target.classList.add('fc-day-today');

  }
  const renderListEvent = (eventInfo) => {
    const scrimEvent = eventInfo.event.extendedProps.data;
    const isCollegiate = scrimEvent.collegiate;
    const steamId = scrimEvent.steamId;
    const eventId = scrimEvent.id;
    const alreadyRequested = scrimEvent.scrimRequests && scrimEvent.scrimRequests.length && props.userInfo && scrimEvent.scrimRequests.find(r => r.steamId === props.userInfo.steamId);
    const profileUrl = `https://steamcommunity.com/profiles/${steamId}`
    const serverList = "[" + scrimEvent.servers.split(',').map(sid => ServerList.find(s => s.id === Number(sid)).title).join(', ') + "]"
    return (
      <div>
        <span className={classes.playerIdentifier}>
          <a href={profileUrl} target="_blank" rel="noopener noreferrer">{eventInfo.event.title}</a> {isCollegiate && <i>Collegiate</i>} {serverList}
        </span>
        {((isLoading && <CircularProgress className={classes.loadingIndicator} size={25} />)) || 
          <Button className={classes.requestButton} variant="outlined" color="primary" size="small" onClick={()=> requestEvent(eventId)} disabled={!!alreadyRequested}>
            {(alreadyRequested && `Request Pending`) || `Send Request`}
          </Button>
        }
      </div>
    )
  }

  const renderCalendarEvent = (eventInfo) => {
    return (<>
      <div className="fc-daygrid-event-dot"></div>
      <div className="fc-event-time">{eventInfo.timeText}</div>
    </>)
  }

  return (
    <Paper elevation={3}>
      <ListItem>
        <h5 className='scheduleHeader'>Available Scrims</h5>
      </ListItem>

      <Grid className={classes.styledGrid} container>
        <Grid item md style={{marginRight: '5px'}}>
          <FullCalendar
            ref={calendarComponentRef}
            plugins={[ dayGridPlugin, interactionPlugin ]}
            initialView='dayGridMonth'
            eventClick={onEventClick}
            eventClassNames={classes.event}
            eventContent={renderCalendarEvent}
            dayMaxEvents={2}
            dateClick={onDateClick}
            now={selectedDate}
            height={500}
            headerToolbar={{
              start:   'title',
              center: '',
              end:  'prev,next'
            }}
          />
        </Grid>
        <Grid item md style={{marginLeft: '5px'}}>
          <FullCalendar
            ref={listComponentRef}
            plugins={[ listPlugin ]}
            initialView='listDay'
            dateClick={onDateClick}
            eventContent={renderListEvent}
            now={selectedDate}
            height={500}
            headerToolbar={{
              start:   'title',
              center: '',
              end:  ''
            }}
          />
        </Grid>
      </Grid>
    </Paper>
  );
}

const useStyles = makeStyles({
  event:{
    pointerEvents: 'none'
  },
  requestButton:{
    fontSize: '11px'
  },
  playerIdentifier:{
    marginRight: '5px'
  },
  styledGrid:{
    '& .fc-toolbar':{
      fontSize: '12px',
      height: '32px',
      marginBottom: '8px',
      '& .fc-toolbar-title':{
        fontSize: '20px',
        marginLeft: '15px'
      }
    },
    '& .fc-daygrid-day':{
      cursor: 'pointer'
    },
    '& .fc-daygrid-day-top':{
      position: 'absolute',
      right: 0
    },
    '& .fc-daygrid-day-events':{
      position: 'absolute !important',
      marginTop: '3px'
    },
    '& .fc-event-time':{
      lineHeight: 1.25
    }
  },
  loadingIndicator:{
    verticalAlign: 'middle',
    marginLeft: '5px'
  }
});