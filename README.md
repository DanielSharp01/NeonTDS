# NeonTDS

Neon TDS is a simple vector based Top Down Shooter game written on the UWP platform with Win2D as a university project.
I lead a team of 3 people and wrote most of the networking and graphics related code.

The networking is a simple UDP packet based authorative server implementation that takes the user input as packets to the server and responds with the actual position information.
Client side prediction is implemented so as to be able to play the game with a higher than 0 ping, but this prediction is getting out of sync by quite a lot if higher latency is experienced.
No real lag compensation was implemented due to lack of experience in the matter and lack of time.

If you can read Hungarian a detailed documentation that was handed in for the course can be found in documentation.pdf.
