using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using IConnect4Player;

namespace MarkPlayer
{
    [Export(typeof(IGamePlayer))]
    public class MarkPlayer : IGamePlayer
    {
        private GameStateModel gameState;
        private const int DefenseLevel = 2;  // How many can they get away with?
        private Position defaultPositionForGame;

        public string PlayerName()
        {
            return "Mark";
        }

        public void NewGame()
        {
            gameState = new GameStateModel();
            defaultPositionForGame = PickRandomSpot();
        }

        public Position MakeFirstMove()
        {
            // Let's not be predictable. Or at least predictably unpredicatable.
            return CommitMove(defaultPositionForGame);
        }

        public Position Move(Position opponentsLastMove)
        {
            // Record the enemy's last move before making decisions...!
            RecordStateOfGame(opponentsLastMove, true);

            var riskedCol = DetermineVerticalRisk();
            if (riskedCol != null)
                return CommitMove(riskedCol.GetValueOrDefault());

            riskedCol = DetermineHorizontalRisk();
            if (riskedCol != null)
                return CommitMove(riskedCol.GetValueOrDefault());

            // Need new column?
            if ( (! IsPositionWinnableOnThisColumn(defaultPositionForGame)) || 
                gameState.IsFullUp(defaultPositionForGame))
            {
                RecalculateDefaultPosition();
            }

            // Crap!!
            var whereNext = defaultPositionForGame;
            if (gameState.IsFullUp(defaultPositionForGame))
            {
                while(gameState.IsFullUp(whereNext))
                {
                    whereNext = PickRandomSpotExcludingFulls();
                }
            }

            return CommitMove(whereNext);
        }


        private void RecordStateOfGame(Position column, bool enemyMove)
        {
            var whoDidIt = (enemyMove) ? PlayerDesignation.Enemy : PlayerDesignation.Me;
            gameState.RecordEntryInColumn(column, whoDidIt);
        }

        private Position CommitMove(Position pos)
        {
            RecordStateOfGame(pos, false);
            return pos;
        }

        // Identify any cols which are at risk of falling to the enemy.
        private Position? DetermineVerticalRisk()
        {
            for (int i = 1; i <= 7; i++) // loop over cols
            {
                var riskCounter = 0;
                var currentStateOfCol = gameState.GetColumnEntries(i.ToPositionValue());
                foreach (var cellState in currentStateOfCol)
                {
                    if (cellState == PlayerDesignation.Enemy)
                        riskCounter++;
                    else if (cellState == PlayerDesignation.Me)
                        riskCounter = 0;
                }

                if (riskCounter >= DefenseLevel && currentStateOfCol.Count <= 5)
                    return i.ToPositionValue();
            }

            return null;
        }

        private Position? DetermineHorizontalRisk()
        {
            for (int row = 1; row <= 6; row++)  // loop over horiz rows
            {
                var riskForThisRow = 0;
                
                for (int col = 1; col <= 7; col++) // over cols
                {
                    var actualCol = col.ToPositionValue();

                    var currentColumn = gameState.GetColumnEntries(actualCol);
                    if (currentColumn.Count >= row)
                    {
                        if (currentColumn[row - 1] == PlayerDesignation.Enemy)
                            riskForThisRow++;
                        else if (currentColumn[row - 1] == PlayerDesignation.Me)
                            riskForThisRow = 0;
                    }
                    else
                        riskForThisRow = 0;
                }

                // No checking for whether this is the correct place, but lets find any col
                // which has a free position on this row. If not, forget it
                if (riskForThisRow >= DefenseLevel)
                {
                    for (int col = 1; col <= 7; col++) // over cols
                    {
                        var currentColumnAgain = gameState.GetColumnEntries(col.ToPositionValue());
                        if (currentColumnAgain.Count == row - 1)
                            return col.ToPositionValue();
                    }
                }
            }

            return null;
        }

        private bool IsPositionWinnableOnThisColumn(Position column)
        {
            var columnRecord = gameState.GetColumnEntries(column);

            if (columnRecord.Count > 0)
            {
                // Is enemy occupying top thing leaving not enough room?
                if ( (columnRecord[columnRecord.Count - 1] == PlayerDesignation.Enemy) &&
                     ((columnRecord.Count - 1) >= 3) )
                    return false;

                var rowOfLastEnemyInCol = columnRecord.LastIndexOf(PlayerDesignation.Enemy) + 1;
                if (rowOfLastEnemyInCol >= 2)
                    return true;
            }

            // Um... maybe or maybe not possible? Not sure so whatever.
            return true;
        }

        private Position PickRandomSpot()
        {
            Random randomator = new Random();
            var whereToGoNext = randomator.Next(1, 8);
            return whereToGoNext.ToPositionValue();
        }

        private void RecalculateDefaultPosition()
        {
            defaultPositionForGame = PickRandomSpotExcludingFulls();
        }

        private Position PickRandomSpotExcludingFulls()
        {
            var fulls = gameState.GetFullColumns();

            Position? whereNow = null;
            while (whereNow == null || fulls.Contains(whereNow.GetValueOrDefault()))
            {
                whereNow = PickRandomSpot();
            }

            return whereNow.GetValueOrDefault();
        }
    }

    public static class PositionHelpers
    {
        public static int ToInt(this Position position)
        {
            switch (position)
            {
                case Position.One:
                    return 1;
                case Position.Two:
                    return 2;
                case Position.Three:
                    return 3;
                case Position.Four:
                    return 4;
                case Position.Five:
                    return 5;
                case Position.Six:
                    return 6;
                case Position.Seven:
                    return 7;
                default:
                    return 1;
            }
        }

        public static Position ToPositionValue(this int i)
        {
            switch (i)
            {
                case 1:
                    return Position.One;
                case 2:
                    return Position.Two;
                case 3:
                    return Position.Three;
                case 4:
                    return Position.Four;
                case 5:
                    return Position.Five;
                case 6:
                    return Position.Six;
                case 7:
                    return Position.Seven;
                default:
                    return Position.One;
            }
        }
    }
}
