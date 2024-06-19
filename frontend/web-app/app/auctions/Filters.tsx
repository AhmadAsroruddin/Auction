import React from 'react'
import { ButtonGroup } from 'flowbite-react';
import { Button } from 'flowbite-react';

type Props={
    pageSize:number,
    setPageSize:(size:number)=>void
};

const pageSizeButtons = [4, 8];

export default function Filters({pageSize, setPageSize}:Props) {
  return (
    <div className='flex justify-between items-center mb-4'>
        <div>
            <span className='uppercase mr-7'>Page Size</span>  
            <ButtonGroup>
                {pageSizeButtons.map((value, i) =>(
                    <Button 
                        key={i} 
                        onClick={()=>setPageSize(value)} 
                        color={`${pageSize === value ?'red':'gray'}`}
                        className='focus:ring-0-red'
                    >
                        {value}
                    </Button>
                ))}
            </ButtonGroup>
        </div>
    </div>
  )
}
