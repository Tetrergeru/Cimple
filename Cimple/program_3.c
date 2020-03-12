u64 f(u64 x)
{
	return (x + x);
}

u64 main()
{
	u64 x;
	u64 t1;
	(t1 = (&x));
	(x = ((1 + 1) + 1));
	printd(x);
}