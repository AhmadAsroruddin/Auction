import Image from "next/image";
import Listing from './auctions/Listing';

export default function Home() {
  return (
    <div className="p-10">
      <Listing />
    </div>
  );
}
