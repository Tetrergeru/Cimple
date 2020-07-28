#extern <WSAStartup>
#extern <WSACleanup>
#extern <WSAGetLastError>
#extern <getaddrinfo>
#extern <freeaddrinfo>
#extern <socket>
#extern <closesocket>
#extern <printf>
#extern <scanf>
#extern <VirtualAlloc>
#extern <VirtualFree>
#extern <bind>
#extern <listen>
#extern <accept>
#extern <recv>
#extern <send>
#extern <shutdown>

u64 print_number(u64 x)
{
	u64 string;
	(string = 0x000a7825);
	return printf((&string), x);
}

u64 print_sth()
{
	print_number(0x42);
}

u64 cp(u64 src, u64 dest, u64 length) 
{
	u64 adr;
	(adr = 0);
	while ((length > 0))
	{
		((*(dest + adr)) = (*(src + adr)));
		(adr += 8);
		(length -= 1);
	}
}

u64 copy(u64 arr, u64 length)
{
	u64 l_arr[1];
	cp(arr, l_arr, length);
}

u64 read_number()
{
	u64 x;
	u64 string;
	(string = 0x006425);
	scanf((&string), (&x));
	return x;
}

u64 print_array(u64 arr, u64 length)
{
	u64 adr;	u64 s;
	(adr = 0);	(s = 0);
	
	while ((length > 0))
	{
		print_number((*(arr + adr)));
		(adr += 4);
		(length -= 1);
	}
}

u64 print_error(u64 number)
{
	u64 text;
	(text = 0x0020726f727265);
	printf((&text));
	print_number(number);
}

u64 print_recieved(u64 number)
{
	u64 text;
	(text = 0x0064766372);
	printf((&text));
	print_number(number);
}

u64 print_sent(u64 number)
{
	u64 text;
	(text = 0x00746e6573);
	printf((&text));
	print_number(number);
}

u64 malloc(u64 size)
{
	return VirtualAlloc(0, size, 0x3000, 0x04);
}

u64 free(u64 ptr)
{
	return VirtualFree(ptr, 0, 0x8000);
}

u64 div(u64 what, u64 by)
{
	u64 count;
	(count = 0);
	while((what >= 0))
	{
		(what -= by);
		(count += 1);
	}
	return (count - 1);
}

u64 main()
{
	u64 default_port;
	// "27015"
	//(default_port = 0x003531303732);
	(default_port = 0x003434343434);
	u64 newLine;
	(newLine = 0x000a);
	printf((&default_port));
	printf((&newLine));
	
	u64 iResult;
	// Some connection data
	u64 wsaData;
	(wsaData = malloc(400));
	
	u64 ListenSocket;
	(ListenSocket  = (~0));
	u64 ClientSocket;
	(ClientSocket  = (~0));
	
	u64 result;
	(result = 0);
	u64 hints;
	(hints = malloc(48));
	
	u64 iSendResult;
	// Buffer length
	u64 recvbuflen;
	(recvbuflen = 512);
	// Buffer
	u64 recvbuf;
	(recvbuf = malloc(recvbuflen));
	
	
	// hints.ai_family = AF_INET // = 2
	// hints.ai_flags = AI_PASSIVE // = 1
	((*hints) = 0x0000000200000001);
	// hints.ai_socktype = SOCK_STREAM // = 1
    // hints.ai_protocol = IPPROTO_TCP // = 6
	((*(hints + 8)) = 0x0000000600000001);
	 
	 // Initialize Winsock
	(iResult = WSAStartup(514, wsaData));
	if (iResult)
	{
		print_error(1);
		print_number(iResult);
		return 1;
	}
	else 
	{
		print_number(1);
	}
	
	// Resolve the server address and port
	(iResult = getaddrinfo(0, (&default_port), hints, (&result)));
	if (iResult)
	{
		print_error(2);
		print_number(iResult);
        WSACleanup();
		return 1;
	}
	else
	{
		print_number(2);
	}
	
	// Create a SOCKET for connecting to server
	// socket(result->ai_family, result->ai_socktype, result->ai_protocol)
	//socket(2, 1, 6);
	(ListenSocket = socket(2, 1, 6));
	if ((ListenSocket == (~0)))
	{
		print_error(WSAGetLastError());
       freeaddrinfo(result);
        WSACleanup();
        return 1;
    }
	else
	{
		print_number(3);
	}
	
	// Setup the TCP listening socket
    (iResult = bind( ListenSocket, (*(result + 32)), (*(result + 16))));
    if ((iResult == (~0)))
	{
		print_error(WSAGetLastError());
        freeaddrinfo(result);
        closesocket(ListenSocket);
        WSACleanup();
        return 1;
    }
	else
	{
		print_number(4);
	}
	
    freeaddrinfo(result);
	
	(iResult = listen(ListenSocket, 128));
    if ((iResult == (~0)))
	{
		print_error(WSAGetLastError());
        closesocket(ListenSocket);
        WSACleanup();
        return 1;
    }
	else
	{
		print_number(5);
	}
	
	// Accept a client socket
    (ClientSocket = accept(ListenSocket, 0, 0));
    if ((ClientSocket == (~0)))
	{
		print_error(WSAGetLastError());
        closesocket(ListenSocket);
        WSACleanup();
        return 1;
    }
	else
	{
		print_number(6);
	}

    // No longer need server socket
    closesocket(ListenSocket);
	
	// Receive until the peer shuts down the connection
	(iResult = 1);
    while ((iResult > 0))
	{
        (iResult = recv(ClientSocket, recvbuf, recvbuflen, 0));
        if ((iResult > 0))
		{
            print_recieved(iResult);

			copy(recvbuf, div(iResult, 8));

			// Echo the buffer back to the sender
            (iSendResult = send( ClientSocket, recvbuf, iResult, 0 ));
            if ((iSendResult == (~0)))
			{
				print_error(WSAGetLastError());
                closesocket(ClientSocket);
                WSACleanup();
                return 1;
            }
			else
			{
				print_sent(iSendResult);
			}
        }
        else
		{
			if ((iResult == 0))
			{
				print_number(7);
			}
			else 
			{
				print_error(WSAGetLastError());
				closesocket(ClientSocket);
				WSACleanup();
				return 1;
			}
		}
    }
	
	print_number(8);
				
	(iResult = shutdown(ClientSocket, 1));
    if ((iResult == (~0)))
	{
		print_error(WSAGetLastError());
        closesocket(ClientSocket);
        WSACleanup();
        return 1;
    }
	else
	{
		print_number(8);
	}
	
	// Cleanup
    closesocket(ClientSocket);
    WSACleanup();
		
	free(wsaData);
	free(hints);
	free(recvbuf);
}