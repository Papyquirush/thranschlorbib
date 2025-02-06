﻿Imports System.Net
Imports System.IO
Imports System.Text

Module Program
    Sub Main()
        Dim listener As New HttpListener()
        listener.Prefixes.Add("http://localhost:5000/")
        listener.Start()
        Console.WriteLine("Serveur en écoute sur http://localhost:5000/")

        While True
            Dim context As HttpListenerContext = listener.GetContext()
            Dim request As HttpListenerRequest = context.Request
            Dim response As HttpListenerResponse = context.Response
            Console.WriteLine(request.Url)
            If request.HttpMethod = "POST" AndAlso request.ContentType.StartsWith("multipart/form-data") Then
                ' Lire le fichier envoyé
                Dim boundary As String = request.ContentType.Split("="c)(1)
                Dim reader As New StreamReader(request.InputStream)
                Dim requestBody As String = reader.ReadToEnd()

                ' Trouver le début du fichier
                Dim fileStart As Integer = requestBody.IndexOf("Content-Type: ") + requestBody.Substring(requestBody.IndexOf("Content-Type: ")).IndexOf(vbCrLf) + 4
                Dim fileData As Byte() = System.Text.Encoding.UTF8.GetBytes(requestBody.Substring(fileStart))

                ' Définir le chemin de sauvegarde
                Dim savePath As String = "C:\temp\reçu_fichier.dat" ' Change le chemin si nécessaire
                File.WriteAllBytes(savePath, fileData)

                Console.WriteLine("Fichier reçu et sauvegardé à : " & savePath)

                Dim fileContent As String = File.ReadAllText(savePath, Encoding.UTF8)
                Console.WriteLine("Contenu du fichier : " & vbCrLf & fileContent)

                ' Répondre à Symfony
                Dim responseString As String = "Fichier reçu et sauvegardé avec succès."
                Dim buffer As Byte() = System.Text.Encoding.UTF8.GetBytes(responseString)
                response.ContentLength64 = buffer.Length
                response.OutputStream.Write(buffer, 0, buffer.Length)
            End If
            If request.Url.Equals("http://localhost:5000/troubleshoot") AndAlso request.HttpMethod = "POST" AndAlso request.ContentType.StartsWith("multipart/form-data") Then
                Console.WriteLine(request.Url)

                ' Lire le fichier envoyé
                Dim boundary As String = request.ContentType.Split("="c)(1)
                Dim reader As New StreamReader(request.InputStream)
                Dim requestBody As String = reader.ReadToEnd()

                ' Trouver le début du fichier
                Dim fileStart As Integer = requestBody.IndexOf("Content-Type: ") + requestBody.Substring(requestBody.IndexOf("Content-Type: ")).IndexOf(vbCrLf) + 4
                Dim fileData As Byte() = System.Text.Encoding.UTF8.GetBytes(requestBody.Substring(fileStart))

                ' Répondre aux autres requêtes
                Dim buffer_1 As Byte() = System.Text.Encoding.UTF8.GetBytes("ça marche")
                Dim buffer_2 As Byte() = System.Text.Encoding.UTF8.GetBytes("ça marche aussi")
                response.ContentLength64 = buffer_1.Length
                response.OutputStream.Write(buffer_1, 0, buffer_1.Length)
                response.ContentLength64 = buffer_2.Length
                response.OutputStream.Write(buffer_2, 0, buffer_2.Length)
            End If

            response.OutputStream.Close()
        End While
    End Sub
End Module
