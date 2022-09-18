all:
	@mcs -sdk:4.5 -target:winexe -win32icon:yt-dlp-gui.ico -optimize+ -debug- -out:yt-dlp-gui.exe yt-dlp-gui.cs -r:System.Drawing.dll -r:System.Windows.Forms.dll

clean:
	@rm -f yt-dlp-gui.exe

run:
	@./yt-dlp-gui.exe
