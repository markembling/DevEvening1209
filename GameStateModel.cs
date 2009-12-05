using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IConnect4Player;

namespace MarkPlayer
{
    /// <summary>
    /// Holds the current state of the game
    /// </summary>
    public class GameStateModel
    {
        private Dictionary<Position, List<PlayerDesignation>> gameStateData;

        public GameStateModel()
        {
            // Not nice is it?
            gameStateData = new Dictionary<Position, List<PlayerDesignation>>();
            gameStateData.Add(Position.One, new List<PlayerDesignation>());
            gameStateData.Add(Position.Two, new List<PlayerDesignation>());
            gameStateData.Add(Position.Three, new List<PlayerDesignation>());
            gameStateData.Add(Position.Four, new List<PlayerDesignation>());
            gameStateData.Add(Position.Five, new List<PlayerDesignation>());
            gameStateData.Add(Position.Six, new List<PlayerDesignation>());
            gameStateData.Add(Position.Seven, new List<PlayerDesignation>());
        }

        /// <summary>
        /// Record what just happened
        /// </summary>
        /// <param name="column"></param>
        /// <param name="whoDidIt"></param>
        public void RecordEntryInColumn(Position column, PlayerDesignation whoDidIt)
        {
            gameStateData[column].Add(whoDidIt);
        }

        /// <summary>
        /// Is the specified column full?
        /// </summary>
        /// <param name="columm"></param>
        /// <returns></returns>
        public bool IsFullUp(Position columm)
        {
            return gameStateData[columm].Count >= 6;
        }

        /// <summary>
        /// Returns a list of cols which are full.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Position> GetFullColumns()
        {
            return gameStateData.Where(x => IsFullUp(x.Key)).Select(x => x.Key);
        }

        public List<PlayerDesignation> GetColumnEntries(Position column)
        {
            return gameStateData[column];
        }
    }

    public enum PlayerDesignation
    {
        Me,
        Enemy   // Death to the enemy!
    }
}
