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

u64 helloworld()
{
	u64 s2;
	u64 s1;
	(s2 = 0x0021646c726f);
	(s1 = 0x77202c6f6c6c6548);
	printf((&s1));
	return 0;
}

u64 replace(u64 address, u64 value)
{
	((*address) = value);
}

u64 main()
{
	u64 x;
	helloworld();
	replace((&x), 42);
	printd(mul());
}