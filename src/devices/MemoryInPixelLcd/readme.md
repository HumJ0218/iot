# Memory in Pixel LCDs

Sharp's Memory-in-Pixel LCDs are the perfect solution for compact handheld, wearable, and other small-screen applications that require a battery. Choose from multiple sizes in 64-color, plus monochrome.

The LCD features simple 3-wire Serial I/F connectivity (SI, SCS, SCLK).

Model       | Size  | Resolution | FPS | VDD           | SPI
------------|-------|------------|-----|---------------|-------
LS013B7DH03 | 1.28" | 128×128    | 65  | 3V (3.3V max) | 1.1MHz
LS013B7DH05 | 1.26" | 144×168    | 60  | 3V (3.3V max) | 1.1MHz
LS027B7DH01 | 2.7"  | 400×240    | 20  | 5V (5.5V max) | 2MHz

* LS027B7DH01 can also use 3V power source while recommended value is 5V.

* Interface is all 3V Serial.

## Notice

Due to the capacity limitation of the SPI output buffer, a buffer overflow exception may occur when update the screen with a high pixel count (eg. LS027B7DH01 with Raspberry Pi).

To avoid this problem, you should limit the amount of lines and split one update into multiple updates or enlarge SPI output buffer.

## Documentation

[LS013B7DH03 Datasheet](https://media.digikey.com/pdf/Data%20Sheets/Sharp%20PDFs/LS013B7DH03_Spec.pdf)

[LS013B7DH05 Datasheet](https://media.digikey.com/pdf/Data%20Sheets/Sharp%20PDFs/LS013B7DH05.pdf)

[LS027B7DH01 Datasheet](https://media.digikey.com/pdf/Data%20Sheets/Sharp%20PDFs/LS027B7DH01_Rev_Jun_2010.pdf)

[Product Page](https://www.sharpsecd.com/#/memory-in-pixel-lcds-product)
