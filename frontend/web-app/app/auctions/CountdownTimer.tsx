'use client'

import React from 'react';
import Countdown, { zeroPad } from 'react-countdown';

type Props = {
    auctionEnd: string;
}

const renderer = ({ days, hours, minutes, seconds, completed }:{days:number, hours:number, minutes:number, seconds:number, completed:boolean}) => {
  return(
    <div 
      className={`
        border-2 
        border-white 
        flex 
        justify-center 
        text-white 
        rounded-lg 
        py-1 
        px-1 
        ${completed? 'bg-red-600' : (days===0 && hours <10) ? 'bg-amber-600':'bg-green-600'}`
    }>

      {completed?  <span>Finished</span> : <span>{zeroPad(days)}:{zeroPad(hours)}:{zeroPad(minutes)}:{zeroPad(seconds)}</span>}
    
    </div>
  )
   
};

export default function CountdownTimer({ auctionEnd }: Props) {
  return (
    <div>
      <Countdown date={auctionEnd} renderer={renderer} />
    </div>
  );
}
