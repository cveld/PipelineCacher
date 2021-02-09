using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.ObjectGraphVisitors;
using YamlDotNet.Serialization.TypeInspectors;

namespace PipelineCacher.Tests
{
	// Pasted from https://dotnetfiddle.net/8M6iIE; mentioned on https://github.com/aaubry/YamlDotNet/issues/152
	public class YamlCommentsExample
    {
	
		[Fact]
		public static void TestComments()
		{
			var serializer = new SerializerBuilder()
				.WithTypeInspector(inner => new CommentGatheringTypeInspector(inner))
				.WithEmissionPhaseObjectGraphVisitor(args => new CommentsObjectGraphVisitor(args.InnerVisitor))
				.Build();

			var address = new Address
			{
				street = "123 Tornado Alley\nSuite 16",
				city = "East Westville",
				state = "KS"
			};

			var receipt = new Receipt
			{
				receipt = "Oz-Ware Purchase Invoice",
				date = new DateTime(2007, 8, 6),
				customer = new Customer
				{
					given = "Dorothy",
					family = "Gale"
				},
				items = new Item[]
				{
				new Item
				{
					part_no = "A4786",
					descrip = "Water Bucket (Filled)",
					price = 1.47M,
					quantity = 4
				},
					new Item
				{
					part_no = "E1628",
					descrip = "High Heeled \"Ruby\" Slippers",
					price = 100.27M,
					quantity = 1
				}
				},
				bill_to = address,
				ship_to = address,
				specialDelivery = "Follow the Yellow Brick\n" +
					"Road to the Emerald City.\n" +
					"Pay no attention to the\n" +
					"man behind the curtain."
			};

			var yaml = serializer.Serialize(receipt);
			Console.WriteLine(yaml);

			var deserializer = new DeserializerBuilder().Build();
			deserializer.Deserialize<Receipt>(yaml);
		}
	}

	public class CommentGatheringTypeInspector : TypeInspectorSkeleton
	{
		private readonly ITypeInspector innerTypeDescriptor;

		public CommentGatheringTypeInspector(ITypeInspector innerTypeDescriptor)
		{
			if (innerTypeDescriptor == null)
			{
				throw new ArgumentNullException("innerTypeDescriptor");
			}

			this.innerTypeDescriptor = innerTypeDescriptor;
		}

		public override IEnumerable<IPropertyDescriptor> GetProperties(Type type, object container)
		{
			return innerTypeDescriptor
				.GetProperties(type, container)
				.Select(d => new CommentsPropertyDescriptor(d));
		}

		private sealed class CommentsPropertyDescriptor : IPropertyDescriptor
		{
			private readonly IPropertyDescriptor baseDescriptor;

			public CommentsPropertyDescriptor(IPropertyDescriptor baseDescriptor)
			{
				this.baseDescriptor = baseDescriptor;
				Name = baseDescriptor.Name;
			}

			public string Name { get; set; }

			public Type Type { get { return baseDescriptor.Type; } }

			public Type TypeOverride
			{
				get { return baseDescriptor.TypeOverride; }
				set { baseDescriptor.TypeOverride = value; }
			}

			public int Order { get; set; }

			public ScalarStyle ScalarStyle
			{
				get { return baseDescriptor.ScalarStyle; }
				set { baseDescriptor.ScalarStyle = value; }
			}

			public bool CanWrite { get { return baseDescriptor.CanWrite; } }

			public void Write(object target, object value)
			{
				baseDescriptor.Write(target, value);
			}

			public T GetCustomAttribute<T>() where T : Attribute
			{
				return baseDescriptor.GetCustomAttribute<T>();
			}

			public IObjectDescriptor Read(object target)
			{
				var description = baseDescriptor.GetCustomAttribute<DescriptionAttribute>();
				return description != null
					? new CommentsObjectDescriptor(baseDescriptor.Read(target), description.Description)
					: baseDescriptor.Read(target);
			}
		}
	}

	public sealed class CommentsObjectDescriptor : IObjectDescriptor
	{
		private readonly IObjectDescriptor innerDescriptor;

		public CommentsObjectDescriptor(IObjectDescriptor innerDescriptor, string comment)
		{
			this.innerDescriptor = innerDescriptor;
			this.Comment = comment;
		}

		public string Comment { get; private set; }

		public object Value { get { return innerDescriptor.Value; } }
		public Type Type { get { return innerDescriptor.Type; } }
		public Type StaticType { get { return innerDescriptor.StaticType; } }
		public ScalarStyle ScalarStyle { get { return innerDescriptor.ScalarStyle; } }
	}

	public class CommentsObjectGraphVisitor : ChainedObjectGraphVisitor
	{
		public CommentsObjectGraphVisitor(IObjectGraphVisitor<IEmitter> nextVisitor)
			: base(nextVisitor)
		{
		}
		
		public override bool EnterMapping(IPropertyDescriptor key, IObjectDescriptor value, IEmitter context)
		{
			var commentsDescriptor = value as CommentsObjectDescriptor;
			if (commentsDescriptor != null && commentsDescriptor.Comment != null)
			{
				context.Emit(new Comment(commentsDescriptor.Comment, false));
			}

			return base.EnterMapping(key, value, context);
		}
	}

	public class Address
	{
		public string street { get; set; }
		public string city { get; set; }
		public string state { get; set; }
	}

	public class Receipt
	{
		public string receipt { get; set; }
		public DateTime date { get; set; }
		public Customer customer { get; set; }
		public Item[] items { get; set; }

		[Description("The address for the invoice")]
		public Address bill_to { get; set; }

		[Description("The address where the goods are to be delivered")]
		public Address ship_to { get; set; }

		[Description("Please follow these instructions carefully!")]
		public string specialDelivery { get; set; }
	}

	public class Customer
	{
		public string given { get; set; }
		public string family { get; set; }
	}

	public class Item
	{
		public string part_no { get; set; }
		public string descrip { get; set; }

		[Description("Unit price in USD")]
		public decimal price { get; set; }
		public int quantity { get; set; }
	}

}
