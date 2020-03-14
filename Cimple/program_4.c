u64 mul_v(u64 x, u64 y)
{
	u64 result;
	(result = 0);
	while(((y - 1) >= 0))
	{ 
		(result += x);
		(y -= 1);
	}
	return result;
}

u64 mul_r(u64 x, u64 y)
{
	if ((y == 0))
	{
		return 0;
	}
	else
	{
		return (x + mul_r(x, (y - 1)));
	}
}

u64 main()
{
	//u64 x;
	//(x = 0xffff);
	//(x = (2 == 2));
	//if (x)
	//{
		printd(mul_r(10,5));
	//}
	//else
	//{
	//	printd(42);
	//}
}