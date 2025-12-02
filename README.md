# SmesnoiM_Tema9
1. OpenGL în sine nu opereaza cu imagini, ci cu texturi. Imaginile pot fi încărcate cu ajutorul utilităților externe(stb_image), sau precum va defini programatorul. Acele texturi pot avea canal alpha pentru transparență. Astfel, orice fel de imagine poate fi aplicată.
2. Culoarea obiectului se amestecă cu culoarea setată, dar dacă este setat culoarea albă, atunci culorile nu se vor altera - se va afișa strict culorile texturii obiectului.
3. In calculul cu iluminare externă activă se va combina (prin blending) culoarea obiectului texturat cu lumina din scenă. Pe fiecare pixel se va combina culoarea texelului respectiv cu ecuația de lumina corespunzătoare , având în vedere poziția sursei de lumină , proprietățile ei, proprietățile materialului.
