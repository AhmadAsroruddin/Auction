import React from 'react'
import AuctionCard from './AuctionCard';
import { Auction, PagedResult } from '@/types';

async function getAllData():Promise<PagedResult<Auction>>{
    const data = await fetch('http://localhost:6001/search?pageSize=10');

    if(!data.ok) throw new Error('failed to fetch data')

    return data.json();
}

export default async function Listing() {
    const data =await getAllData();

    return (
        <div className='grid grid-cols-4 gap-6'>
            {data && data.results.map((auction) =>(
                <AuctionCard auction={auction} key={auction.id}/>
            ))}
        </div>
    )
}
