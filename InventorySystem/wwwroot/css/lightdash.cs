using System.ComponentModel;

body {
    font-family: 'Roboto', sans - serif;
margin: 0;
background - color: #f9f9f9;
    color: #333333;
}

/* Animated Dashboard Heading */
h1 {
    margin-left: 70px;
margin - top: 50px;
font - size: 36px;
color: #333333;
    font - weight: 700;
animation: fadeIn 2s ease-in-out;
}

/* Fade-in animation for Dashboard */
@keyframes fadeIn
{
    0% { opacity: 0; transform: translateY(-20px); }
    100% { opacity: 1; transform: translateY(0); }
}

.container {
    width: 100 %;
margin: 0px;
padding: 0 20px;
margin - bottom: 50px;
}

.row {
    display: flex;
gap: 15px;
flex - wrap: wrap;
margin - bottom: 20px;
}

/* Adjusted Chart Container Size */
.chart - container {
background: #ffffff;
    padding: 20px 0;
color: #333333;
    border - radius: 4px;
    box - shadow: 0 4px 10px rgba(0, 0, 0, 0.1);
padding: 20px;
flex: 1 1 calc(25 % -15px);
    max - width: 100 %; /* Set a max-width to control width */
height: 430px; /* Set a height for consistency */
display: flex;
    flex - direction: column;
    justify - content: center; /* Center the charts horizontally */
transition: transform 0.3s ease-in-out, background - color 0.3s ease, opacity 0.3s ease-in-out; /* Adding transition for transform */
opacity: 1; /* Initial opacity for animation */
animation: fadeIn 1s ease-in-out forwards; /* Animation for fade-in */
}

.card {
    background: #ffffff;
    color: #333333;
    border - radius: 4px;
box - shadow: 0 4px 10px rgba(0, 0, 0, 0.1);
padding: 20px;
flex: 1 1 calc(25 % -15px);
min - width: 230px;
transition: flex - grow 0.3s ease-in-out, transform 0.3s ease-in-out, background-color 0.3s ease, opacity 0.3s ease-in-out; /* Adding transition for transform */
opacity: 1; /* Initial opacity for animation */
animation: fadeIn 1s ease-in-out forwards; /* Animation for fade-in */
}

/* Delay the chart containers to make them fade in one by one */
.chart - container:nth - child(1) {
    animation - delay: 0.5s;
}

.chart - container:nth - child(2) {
    animation - delay: 1s;
}

.chart - container:nth - child(3) {
    animation - delay: 1.5s;
}

.card h3
{
    font-size: 18px;
    font-weight: 500;
    margin-bottom: 10px;
    opacity: 0.8;
}

.card p
{
    font-size: 28px;
    font-weight: 700;
}

/* Hover Effects - Scale Effect */
.card: hover {
    flex - grow: 6;
transform: scale(1.9); /* Smooth scale-up on hover */
    background - color: #f0f0f0; /* Light background on hover */
    opacity: 0.9; /* Fade effect */
}

/* Hover Effects - Scale Effect */
.chart - container:hover {
    transform: scale(1.9); /* Smooth scale-up on hover */
background - color: #f0f0f0; /* Light background on hover */
    opacity: 0.9; /* Fade effect */
color: #333333;
}

.card h3, .card p {
    transition: color 0.3s ease-in-out;
}

.card: hover h3, .card: hover p {
color: #000000; /* Darker text color on hover */
}

/* Chart Styling */
.chart - container h3 {
    margin-bottom: 15px;
font - size: 20px;
font - weight: 500;
opacity: 0.8;
animation: fadeIn 1.5s ease-in-out;
}

/* Card Styling */
.card {
    background: #ffffff;
    color: #333333;
}

/* Make sure the canvas is smaller */
canvas {
    max-width: 90 %; /* Ensure the canvas takes the full width of its container */
max - height: 90 %; /* Ensure the canvas takes the full height of its container */
width: 80 %; /* Reduce the width of the canvas to 80% of the container */
height: 80 %; /* Reduce the height of the canvas to 80% of the container */
margin - left: 23px;
}
