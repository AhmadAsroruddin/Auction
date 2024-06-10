import React from 'react'
import AuctionCard from './AuctionCard';

async function getAllData(){
    const data = await fetch('http://localhost:6001/search');

    if(!data.ok) throw new Error('failed to fetch data')

    return data.json();
}

export default async function Listing() {
    const data =await getAllData();

    return (
        <div>
            {data && data.results.map((auction:any) =>(
                <AuctionCard auction={auction}/>
            ))}
        </div>
    )
}
