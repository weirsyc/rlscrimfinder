import React from 'react';
import { Launcher } from 'react-chat-window';

import './chatbox.css'

export const Chatbox = props => {
  const formattedMessages = props.chatLog.map(c => {
    let message = {type: "text", data: {text: c.message }};
    if (c.steamId === props.userInfo.steamId){
      message.author = "me"
    }
    else{
      message.author = props.opponentName
    }
    return message;
  });

  return (
    <div> 
      <Launcher
        agentProfile={{
          teamName: 'Chat with opponent',
          imageUrl: ''
        }}
        isOpen={true}
        onMessageWasSent={props.sendMessage}
        messageList={formattedMessages}
      />
    </div>
  );
}