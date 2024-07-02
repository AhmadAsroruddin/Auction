'use server'

import { Auction, PagedResult } from '@/types';


export default async function getAllData(pageNumber: number = 1,pageSize:number):Promise<PagedResult<Auction>>{
    const data = await fetch(`http://localhost:6001/search?pageSize=${pageSize}&pageNumber=${pageNumber}`);

    if(!data.ok) throw new Error('failed to fetch data')

    return data.json();
}

