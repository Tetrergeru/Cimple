#extern <printf>
#extern <VirtualAlloc>

u64 print_number(u64 x)
{
	u64 string;
	(string = 0x000a756c25);
	return printf((&string), x);
}

u64 print_sth()
{
	print_number(42);
}

u64 print_array(u64 arr, u64 length)
{
	while ((length > 0))
	{
		print_number((*arr));
		(arr += 4);
		(length -= 1);
	}
}

u64 copy(u64 arr, u64 length)
{
	u64 l_arr[5];
	u64 adr;
	(adr = 0);
	while ((length > 0))
	{
		((*(l_arr + adr)) = (*(arr + adr)));
		(adr += 8);
		(length -= 1);
	}
}

u64 mul(u64 x, u64 y)
{
	u64 result;
	(result = 0);
	while((y > 0))
	{ 
		(result += x);
		(y -= 1);
	}
	return result;
}

u64 malloc(u64 size)
{
	return VirtualAlloc(0, size, 0x3000, 0x04);
}

u64 fill(u64 arr, u64 length, u64 value)
{
	u64 adr;
	(adr = 0);
	while ((length > 0))
	{
		((*(arr + adr)) = value);
		(adr += 8);
		(length -= 1);
	}
}

u64 put(u64 arr, u64 i, u64 value)
{
	((*(arr + mul(i, 8))) = value);
}

u64 get(u64 arr, u64 i)
{
	return (*(arr + mul(i, 8)));
}

u64 main()
{
	u64 space[100];
	
	//print_sth();
	
	
	u64 arr;
	(arr = malloc(2000));
	fill(arr, 5, 0x0000000100000002);
	put(arr, 5, 3);
	put(arr, 6, arr);
	put(arr, 7, 1309680);
	
	//put(arr, 8, 0xe85efeffff000000);
	//68 3B 10 40 00 C3
	put(arr, 0, 0xc30040103b68);
	
	//put(arr, 8, 0x48B86F726C642100 );
	//put(arr, 8, 0x0021646c726fb848);
	
	//put(arr, 9, 0x00005048B848656C);
	//put(arr, 9, 0x6c6548b848500000);
	
	//put(arr, 10, 0x6C6F2C2077504883);
	//put(arr, 10, 0x83485077202c6f6c);
	
	//put(arr, 11, 0xEC28488D4C2428E8);
	//put(arr, 11, 0xe828244c8d4828ec);
	
	//put(arr, 12, 0x501E0000);
	//put(arr, 12, 0x00001e50);
	
	//print_array(arr, 16);
	
	copy(arr, 8);
}