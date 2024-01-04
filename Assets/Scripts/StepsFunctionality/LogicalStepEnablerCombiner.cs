using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class LogicalStepEnablerCombiner : StepEnabler
{
    public enum Operator
    {
        AND,
        OR,
        NAND,
        NOR
    }

    private abstract class LogicConditionAsserter
    {
        public abstract bool AssertCondition(List<StepEnabler> stepEnablers);
    }

    private class OrAsserter : LogicConditionAsserter
    {
        public override bool AssertCondition(List<StepEnabler> stepEnablers)
        {
            return stepEnablers.Any(x => x.Enabled = true);
        }
    }

    private class AndAsserter : LogicConditionAsserter
    {
        public override bool AssertCondition(List<StepEnabler> stepEnablers)
        {
            return stepEnablers.All(x => x.Enabled == true);
        }
    }

    private class NandAsserter : LogicConditionAsserter
    {
        public override bool AssertCondition(List<StepEnabler> stepEnablers)
        {
            return !stepEnablers.All(x => x.Enabled == true);
        }
    }

    private class NorAsserter : LogicConditionAsserter
    {
        public override bool AssertCondition(List<StepEnabler> stepEnablers)
        {
            return !stepEnablers.Any(x => x.Enabled = true);
        }
    }

    private class AsserterFactory
    {
        public static LogicConditionAsserter CreateAsserterObject(Operator operatorType)
        {
            switch (operatorType)
            {
                case Operator.AND:
                    return new AndAsserter();
                case Operator.OR:
                    return new OrAsserter();
                case Operator.NAND:
                    return new NandAsserter();
                case Operator.NOR:
                    return new NorAsserter();
                default:
                    return null;

            }
        }
    }

    [Header("Logical Step Enabler Combiner")]
    [SerializeField] private List<StepEnabler> stepEnablers = default;

    [SerializeField] private Operator operatorType = default;

    private LogicConditionAsserter asserter = default;

    protected override void initialize()
    {
        asserter = AsserterFactory.CreateAsserterObject(operatorType);
    }

    protected override void onUpdate()
    {
        AssertCondition();
    }

    public void AssertCondition()
    {
        Enabled = asserter.AssertCondition(stepEnablers);
    }

    public void SetAllEnabledState(bool toState)
    {
        stepEnablers.ForEach(x => x.Enabled = toState);
    }
}