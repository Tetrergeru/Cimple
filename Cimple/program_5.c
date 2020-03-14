u64 sum(u64 arr, u64 length)
{
	u64 adr;
	u64 s;
	(adr = 0);
	(s = 0);
	while ((length > 0))
	{
		(s += (*(arr + adr)));
		(adr += 8);
		(length -= 1);
	}
	return s;
}

u64 sum_r(u64 arr, u64 length)
{
	if ((length == 0))
	{
		return 0;
	}
	else
	{
		return ((*arr) + sum_r((arr + 8), (length - 1)));
	}
}

u64 malloc(u64 size)
{
	return VirtualAlloc(0, size, 0x00003000, 0x04);
}

u64 main()
{
	u64 array;
	(array = malloc(40));
	
	u64 i; u64 adr;
	(i = 0); (adr = 0);
	
	while ((i < 5))
	{
		((*(array + adr)) = (i + 1));
		(i += 1);
		(adr += 8);
	}
	
	printd(sum_r(array, 5));
}