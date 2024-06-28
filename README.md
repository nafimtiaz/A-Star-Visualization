# A-Star-Visualization

This Unity application provides an interactive visualization of the A* (A-star) algorithm, a popular pathfinding and graph traversal technique used in computer science and artificial intelligence. Users can observe how the algorithm navigates through a grid to find the shortest path from a start point to an end point, avoiding obstacles along the way. The visualizer offers adjustable parameters and real-time updates, making it an excellent tool for both educational purposes and a deeper understanding of the A* algorithm's mechanics.

https://github.com/nafimtiaz/A-Star-Visualization/assets/6371642/3a2aa4a7-0f3f-467b-8ae8-ee1f73f350b7

## Features

- **Interactive Node Placement:**
  - Place start and target points.
  - Add and erase obstacles on the grid.

- **Pathfinding Controls:**
  - Play the pathfinding steps in real time.
  - Step through the pathfinding process one step at a time.
  - View the overall result with a single click.

- **Visual Indicators:**
  - Nodes are color-coded to indicate their status:
    - **Open Nodes(bright red)**: Nodes yet to be evaluated.
    - **Closed Nodes(dark red)**: Nodes that have already been evaluated.
    - **Final Path(green) Nodes**: Nodes that form the final path.

- **Node Details:**
  
  <img width="268" alt="node_info" src="https://github.com/nafimtiaz/A-Star-Visualization/assets/6371642/e49e4e11-661c-4e03-b5d4-4bd11d7a72ab"><br>

  - Each node displays its `hCost`, `gCost`, and `fCost`.
  - Arrows indicate the direction of parent nodes.

