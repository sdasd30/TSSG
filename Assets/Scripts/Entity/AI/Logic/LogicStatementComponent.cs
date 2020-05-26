using System.Collections;
using System.Collections.Generic;

public class LogicStatementComponent 
{
    public bool NOT = false;
    public Noun Noun;
    public enum LogicStatementComponentValue { AND, OR , XOR};
    public int Group;
    public LogicStatementComponentValue LogicOperator;
}
