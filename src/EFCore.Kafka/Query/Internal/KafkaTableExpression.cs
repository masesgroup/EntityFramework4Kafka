/*
*  Copyright 2022 MASES s.r.l.
*
*  Licensed under the Apache License, Version 2.0 (the "License");
*  you may not use this file except in compliance with the License.
*  You may obtain a copy of the License at
*
*  http://www.apache.org/licenses/LICENSE-2.0
*
*  Unless required by applicable law or agreed to in writing, software
*  distributed under the License is distributed on an "AS IS" BASIS,
*  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
*  See the License for the specific language governing permissions and
*  limitations under the License.
*
*  Refer to LICENSE for more information.
*/

namespace MASES.EntityFrameworkCore.Kafka.Query.Internal;

public class KafkaTableExpression : Expression, IPrintableExpression
{
    public KafkaTableExpression(IEntityType entityType)
    {
        EntityType = entityType;
    }

    public override Type Type
        => typeof(IEnumerable<ValueBuffer>);

    public virtual IEntityType EntityType { get; }

    public sealed override ExpressionType NodeType
        => ExpressionType.Extension;

    protected override Expression VisitChildren(ExpressionVisitor visitor)
        => this;

    void IPrintableExpression.Print(ExpressionPrinter expressionPrinter)
        => expressionPrinter.Append(nameof(KafkaTableExpression) + ": Entity: " + EntityType.DisplayName());
}
