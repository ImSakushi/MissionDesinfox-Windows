#!/usr/bin/env python3
# -*- coding: utf-8 -*-

import os
import re
import glob
import requests

def download_images_from_csv(data_dir):
    """
    Pour chaque fichier CSV dans data_dir, 
    télécharge toutes les images référencées via des URLs https://storage.googleapis.com
    et les enregistre dans data_dir/Images.
    """
    # Dossier de sortie
    images_dir = os.path.join(data_dir, "Images")
    os.makedirs(images_dir, exist_ok=True)

    # Regex pour capturer les URLs
    url_pattern = re.compile(r'https://storage\.googleapis\.com/[^\s,"\'<>]+')

    seen_urls = set()

    # Parcours de tous les CSV
    for csv_path in glob.glob(os.path.join(data_dir, "*.csv")):
        print(f"→ Lecture de {csv_path}")
        with open(csv_path, encoding="utf-8") as f:
            text = f.read()
        urls = url_pattern.findall(text)

        for url in urls:
            if url in seen_urls:
                continue
            seen_urls.add(url)

            # Extraire un nom de fichier propre (sans paramètres GET)
            filename = os.path.basename(url.split('?')[0])
            dest_path = os.path.join(images_dir, filename)

            print(f"Téléchargement : {url}")
            try:
                r = requests.get(url, stream=True, timeout=30)
                r.raise_for_status()
                with open(dest_path, "wb") as img_f:
                    for chunk in r.iter_content(chunk_size=8192):
                        img_f.write(chunk)
                print(f"  ✔ Enregistré sous {dest_path}")
            except Exception as e:
                print(f"  ✘ Échec du téléchargement : {e}")

    print("\nTerminé : toutes les images ont été téléchargées dans", images_dir)


if __name__ == "__main__":
    # À adapter si nécessaire :
    data_directory = r"C:\Users\Pablo\Desktop\MissionDesinfox\Assets\Resources\Data"
    download_images_from_csv(data_directory)
