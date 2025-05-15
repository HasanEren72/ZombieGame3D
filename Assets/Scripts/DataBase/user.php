<?php
    error_reporting(0);
                            //hostad� //adminad�//adminsifre //db
    $baglanti = new mysqli("localhost","root","","zombiegame");

    // Check connection
    if ($baglanti->connect_error) {
      echo"Connection failed: " . $baglanti->connect_error;
    }

    if($_POST['unity']=="kayitOlma"){

      $kullaniciAdi = $_POST['kullaniciAdi'];   //
      $sifre =md5(sha1(md5(sha1($_POST["sifre"]))));  //MD5 ve Sha1 ile �ifrelenmi� �ifreyi atama
      $tarih = date("Y.m.d"); //tarih ve saat bilgisini al�r 
  
      //$kulad=htmlspecialchars(strip_tags($_POST["kullaniciAdi"]));  
      //$sifre=md5(sha1(md5(sha1(htmlspecialchars(strip_tags($_POST["sifre"]))))));  MD5 �ifreleme
      //$mail=htmlspecialchars(strip_tags($_POST["mail"]));

      $sorgu = "insert into kayitlar(kul_adi,password,kayitTarihi) values ('$kullaniciAdi','$sifre','$tarih')";

      $sorgusonucu = $baglanti -> query($sorgu);

      if ($sorgusonucu)
      {
        echo "kayit basarili";
      }
      else
      {    
        echo"l�tfen farkl� bir kullan�c� ad� se�iniz";
      }
    }

    if($_POST['unity']=="girisYapma"){

      $kullaniciAdi = $_POST['kullaniciAdi'];
      $sifre = md5(sha1(md5(sha1($_POST["sifre"]))));  //MD5 ve Sha1 ile �ifrelenmi� �ifreyi atama

      $sorgu = "select * from kayitlar where kul_adi='$kullaniciAdi' and password='$sifre' ";

      $sorgusonucu = $baglanti->query($sorgu);

      if($sorgusonucu-> num_rows>0){

        echo "giris başarili"; 
      }
      else{

        echo "giris basarisiz";
      }
    }

    if ($_POST['unity'] == "Tum_Skor_verileri_cek") {
        $kullaniciAdi = $_POST['kullaniciAdi'];
        $sifre = md5(sha1(md5(sha1($_POST["sifre"]))));  // Şifreleme işlemi

        $sorgu = "SELECT puan, altin_toplam, elmas FROM skorlar WHERE id=(SELECT id FROM kayitlar WHERE kul_adi='$kullaniciAdi' AND password='$sifre')";
        $sorgusonucu = $baglanti->query($sorgu);

        if ($sorgusonucu->num_rows > 0) 
        {
          $sonuc = mysqli_fetch_assoc($sorgusonucu);
          echo $sonuc['puan'] . "|" . $sonuc['altin_toplam'] . "|" . $sonuc['elmas'];
        } else
        {       
          echo "basarisiz";
        }
    }
    if ($_POST['unity'] == "SilahCekme") {

      $kullaniciAdi = $_POST['kullaniciAdi'];
      $sifre = md5(sha1(md5(sha1($_POST["sifre"]))));  //MD5 ve Sha1 ile �ifrelenmi� �ifreyi atama
      $silahAdi = $_POST['silahAdi'];

      $TumSilahlar = ["ump45","ak47", "m416", "m16a4"];

      if (!in_array( $silahAdi, $TumSilahlar)) //silahadı tumsilahlar dizisinde varmı
      {
        echo "gecersiz_silah";
        exit;
      }

      $sorgu = "SELECT $silahAdi FROM `silahlar` WHERE id=(SELECT id FROM kayitlar WHERE kul_adi='$kullaniciAdi' AND password='$sifre')";

      $sorgusonucu = $baglanti->query($sorgu);

      if ($sorgusonucu->num_rows > 0) 
      {
        $srow = mysqli_fetch_assoc($sorgusonucu);
        echo $srow[$silahAdi];
      } else 
      {
        echo "basarisiz";
      }
    }

    if ($_POST['unity'] == "YeniBolumKilitCekme") {

      $kullaniciAdi = $_POST['kullaniciAdi'];
      $sifre = md5(sha1(md5(sha1($_POST["sifre"]))));  //MD5 ve Sha1 ile �ifrelenmi� �ifreyi atama

      $sorgu = "SELECT gecti  FROM `bolum2` WHERE id=(select id from kayitlar where  kul_adi='$kullaniciAdi' and password ='$sifre' ) ";

      $sorgusonucu = $baglanti->query($sorgu);

      if ($sorgusonucu->num_rows > 0) {

        $srow = mysqli_fetch_assoc($sorgusonucu);
        echo $srow["gecti"];
      } else {

        echo "basarisiz";      
      }
    }

    if ($_POST['unity'] == "YeniBolumKilitAcma") {  // veritaban�na g�ncellenmi� �ekilde ekler

      $kullaniciAdi = $_POST['kullaniciAdi'];
      $sifre = md5(sha1(md5(sha1($_POST["sifre"]))));  //MD5 ve Sha1 ile �ifrelenmi� �ifreyi atama

      $sorgu = " UPDATE  bolum2 SET  gecti=1 WHERE id=(select id from kayitlar where  kul_adi='$kullaniciAdi' and password ='$sifre' ) ";

      $sorgusonucu = $baglanti->query($sorgu);

      if ($sorgusonucu) {
        echo "Yeni bolum kilit acma  Basarili";
      } else {
        echo "Yeni bolum kilit acma  Basarisiz!";
      }
    }

    if($_POST['unity']=="ilk_skorlar_ekleme"){  // kayıt olunca ilk skorlar eklenir
        
          $kullaniciAdi = $_POST['kullaniciAdi'];
          $sifre = md5(sha1(md5(sha1($_POST["sifre"]))));  //MD5 ve Sha1 ile �ifrelenmi� �ifreyi atama

          $puan = 0;
          $toplamaltin =0;
          $top_elmas =0;

        //$sorgu = " INSERT INTO skorlar(id ,puan, altin_toplam,elmas) VALUES((select id=(select id from kayitlar where  kul_adi='$kullaniciAdi' and password ='$sifre' ) ),$puan, $toplamaltin, $top_elmas) ";
            $sorgu = "INSERT INTO skorlar(id ,puan, altin_toplam,elmas)   
            SELECT id, 0, 0, 0 FROM kayitlar WHERE kul_adi='$kullaniciAdi' and password ='$sifre' "; //ilk skorlar ekler

            $sorgu2 = "INSERT INTO silahlar(id ,ak47, m416, m16a4 ,ump45) 
            SELECT id, 0, 0, 0,1 FROM kayitlar WHERE kul_adi='$kullaniciAdi' and password ='$sifre' ";//ilk silahlar ekler sadece ump45 olacak

            $sorgu3 = "INSERT INTO bolum2(id ,gecti) 
            SELECT id,0 FROM kayitlar WHERE kul_adi='$kullaniciAdi' and password ='$sifre' ";//ilk bolum 2 kilitli olarak g�sterecek

            $sorgusonucu = $baglanti->query($sorgu);
            $sorgusonucu41 = $baglanti->query($sorgu2);
            $sorgusonucu42 = $baglanti->query($sorgu3);

          if($sorgusonucu){
            echo "iLK skorlar ve ilk silahlar kayit etme basarili :";
          }
          else{
            echo "iLK skorlar ve ilk silahlar kayit etme basarisiz";
          }
        }

    if($_POST['unity']=="Skorlari_Guncelle"){  // veritaban�na g�ncellenmi� �ekilde ekler

      $kullaniciAdi = $_POST['kullaniciAdi'];
      $sifre = md5(sha1(md5(sha1($_POST["sifre"]))));  //MD5 ve Sha1 ile �ifrelenmi� �ifreyi atama

      $puan = (int)$_POST['puan'];
      $toplamaltin = (int)$_POST['toplamAltin'];
      $elmas = (int)$_POST['elmas'];

      $sorgu = " UPDATE  skorlar SET puan=puan+$puan, altin_toplam=altin_toplam+$toplamaltin, elmas=elmas+$elmas WHERE id=(select id from kayitlar where  kul_adi='$kullaniciAdi' and password ='$sifre' ) ";

      $sorgusonucu = $baglanti->query($sorgu);

      if($sorgusonucu){
        echo "skorlar Guncelleme Basarili";
      }
      else{
        echo "skorlar Guncelleme Basarisiz !";
      }
    }

    if ($_POST['unity'] == "Kaynak_Guncelleme") {  // veritaban�na g�ncellenmi� �ekilde ekler

      $kullaniciAdi = $_POST['kullaniciAdi'];
      $sifre = md5(sha1(md5(sha1($_POST["sifre"]))));  //MD5 ve Sha1 ile �ifrelenmi� �ifreyi atama
      $kaynak = $_POST['kaynak']; //altin veya elmas
      $HarcamaMiktari = $_POST['HarcamaMiktari'];

      if ($kaynak == "altin") { //altin ise altin ekleyecek

        $sorgu = " UPDATE  skorlar SET  altin_toplam=altin_toplam-$HarcamaMiktari WHERE id=(select id from kayitlar where  kul_adi='$kullaniciAdi' and password ='$sifre' ) ";
      } 
      else if ($kaynak == "elmas")//elmas ise elmas ekleyecek
      {
        $sorgu = " UPDATE  skorlar SET  elmas=elmas-$HarcamaMiktari WHERE id=(select id from kayitlar where  kul_adi='$kullaniciAdi' and password ='$sifre' ) ";
      }

      $sorgusonucu = $baglanti->query($sorgu);

      if ($sorgusonucu) {
        echo "kaynak guncelleme basarili";
      } else {
        echo "kaynak guncelleme basarisiz";
      }
    }
    
    if ($_POST['unity'] == "silahSatinAlma_Guncelleme") {  // veritaban�na g�ncellenmi� �ekilde ekler

      $kullaniciAdi = $_POST['kullaniciAdi'];
      $sifre = md5(sha1(md5(sha1($_POST["sifre"]))));  //MD5 ve Sha1 ile �ifrelenmi� �ifreyi atama


      if ($_POST['ak47'] != "") { //ak47 g�nderilmi� ise yani sat�n al�nm��sa 

        $sorgu = " UPDATE  silahlar SET ak47=1  WHERE id=(select id from kayitlar where  kul_adi='$kullaniciAdi' and password ='$sifre' ) ";
        $silah = 'ak47';
      } else if ($_POST['m416'] != "") {//m416 g�nderilmi� ise yani sat�n al�nm��sa 

        $sorgu = " UPDATE  silahlar SET m416=1   WHERE id=(select id from kayitlar where  kul_adi='$kullaniciAdi' and password ='$sifre' ) ";
        $silah = 'm416';
      } else if ($_POST['m16a4'] != "") {//m16a4 g�nderilmi� ise yani sat�n al�nm��sa 

        $sorgu = " UPDATE  silahlar SET m16a4=1  WHERE id=(select id from kayitlar where  kul_adi='$kullaniciAdi' and password ='$sifre' ) ";
        $silah = 'm16a4';
      }

      $sorgusonucu = $baglanti->query($sorgu);

      if ($sorgusonucu) {
        echo $silah . " silah satin alma guncerlleme basarili ";
      } else {
        echo "silah satin alma  Guncelleme basarisiz !";
      }
    }

    if ($_POST["unity"] == "Donusturme_Guncelleme") { //donusturme1 i�in g�ncelleme yapar
      $kullaniciAdi = $_POST['kullaniciAdi'];
      $sifre = md5(sha1(md5(sha1($_POST["sifre"]))));  //MD5 ve Sha1 ile �ifrelenmi� �ifreyi atama
      $kaynak = $_POST['kaynak']; //altin veya elmas
      $HarcamaMiktari = $_POST['HarcamaMiktari'];

      if($kaynak == "elmas" && $HarcamaMiktari == 500) { //elmas 500 ise altin 10.000 olacak

        $sorgu = " UPDATE  skorlar SET  elmas=elmas-$HarcamaMiktari,altin_toplam=altin_toplam+10000 WHERE id=(select id from kayitlar where  kul_adi='$kullaniciAdi' and password ='$sifre' ) ";
        $sorgusonucu = $baglanti->query($sorgu);
      } 
      else if($kaynak == "elmas" && $HarcamaMiktari == 100) { //elmas 1000 ise altin 1.000 olacak 
      
        $sorgu = " UPDATE  skorlar SET  elmas=elmas-$HarcamaMiktari,altin_toplam=altin_toplam+1000 WHERE id=(select id from kayitlar where  kul_adi='$kullaniciAdi' and password ='$sifre' ) ";
        $sorgusonucu = $baglanti->query($sorgu);
      } 
      else if($kaynak == "altin" && $HarcamaMiktari == 5000) {

        $sorgu = " UPDATE  skorlar SET  elmas=elmas+200,altin_toplam=altin_toplam-$HarcamaMiktari WHERE id=(select id from kayitlar where  kul_adi='$kullaniciAdi' and password ='$sifre' ) ";
        $sorgusonucu = $baglanti->query($sorgu);
      }

      if ($sorgusonucu) {
        echo "Donusturme guncelleme basarili";
      } else {
        echo "Donusturme guncelleme basarisiz ! Hata: " . mysqli_error($baglanti);
      }

    }
    
?>