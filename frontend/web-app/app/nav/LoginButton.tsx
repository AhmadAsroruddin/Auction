'use client'

import { Button } from 'flowbite-react'
import React from 'react'
import { signIn } from 'next-auth/react';

export default function LoginButton() {
  const handleLogin = () => {
    const absoluteCallbackUrl = `${window.location.origin}/`;
    signIn('id-server', { callbackUrl: absoluteCallbackUrl });
  };

  return (
    <Button outline onClick={handleLogin}>
      Login
    </Button>
  );
}
