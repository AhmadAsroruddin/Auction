'use client'

import React, { useState } from 'react';
import { Pagination } from 'flowbite-react';

type Props ={
    currentPage:number,
    pageCount:number,
    pageChange:(page:number)=>void;
}

export default function AppPagination({currentPage, pageCount, pageChange}:Props) {
    return (
       <>
            <Pagination 
                currentPage={currentPage} 
                onPageChange={pageChange} 
                totalPages={pageCount} 
                layout='pagination'
                showIcons={true}
                className='text-blue-500 mb-5'
            />
       </>
    )
}
