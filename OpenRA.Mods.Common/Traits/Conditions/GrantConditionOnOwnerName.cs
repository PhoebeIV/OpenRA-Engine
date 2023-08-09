#region Copyright & License Information
/*
 * Copyright 2015- OpenRA.Mods.AS Developers (see AUTHORS)
 * This file is a part of a third-party plugin for OpenRA, which is
 * free software. It is made available to you under the terms of the
 * GNU General Public License as published by the Free Software
 * Foundation. For more information, see COPYING.
 */
#endregion

using OpenRA.Traits;

namespace OpenRA.Mods.AS.Traits
{
	[Desc("Grants a condition if the owner belongs to the PlayerName player.")]
	public class GrantConditionOnOwnerNameInfo : TraitInfo
	{
		[GrantedConditionReference]
		[Desc("The condition to grant.")]
		public readonly string Condition = null;

		[Desc("Name of the player.")]
		public readonly string PlayerName = "Creeps";

		public override object Create(ActorInitializer init) { return new GrantConditionOnOwnerName(this); }
	}

	public class GrantConditionOnOwnerName : INotifyCreated, INotifyOwnerChanged
	{
		readonly GrantConditionOnOwnerNameInfo info;

		int token = Actor.InvalidConditionToken;

		public GrantConditionOnOwnerName(GrantConditionOnOwnerNameInfo info)
		{
			this.info = info;
		}

		void INotifyCreated.Created(Actor self)
		{
			if (self.Owner.PlayerName == info.PlayerName)
				GrantCondition(self, info.Condition);
		}

		void GrantCondition(Actor self, string condition)
		{
			token = self.GrantCondition(condition);
		}

		void RevokeCondition(Actor self)
		{
			token = self.RevokeCondition(token);
		}

		void INotifyOwnerChanged.OnOwnerChanged(Actor self, Player oldOwner, Player newOwner)
		{
			if (newOwner.PlayerName == info.PlayerName && token == Actor.InvalidConditionToken)
				GrantCondition(self, info.Condition);
			else if (newOwner.PlayerName != info.PlayerName && token != Actor.InvalidConditionToken)
				RevokeCondition(self);
		}
	}
}
