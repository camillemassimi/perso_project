#!/usr/bin/env python
# -*- coding:Utf-8 -*-
"""
Courant Fanny & Massimi Camille
M2 DS 2021-2022

#######################################
##### Projet PageRank avec mrjob ######
#######################################

MEMO :
- pour lancer le programme il faut ecrire dans le terminal :
            > python nom_fichier.py nom_fichier.txt
- le nombre d'iterations est fixé à 10
- le coefficient c est fixé à 0.15
"""
import re
from mrjob.job import MRJob
from mrjob.step import MRStep
import collections  # pour compter les valeurs dans liste

motif = re.compile("([0-9]+)\s([0-9]+)")  # permet de récupérer les 2 nombres par ligne


class PageRank(MRJob):
    c = 0.15
    nb_pages = 0  # pour compter le nb de pages
    liste_nb_page = []  # la liste des pages
    dico = {}# dico avec en clé la page source (=la page i) et en valeurs les liens sortants c'est-à-dire les pages j que la page i cite (i -> j)
    pages_citeuses = []  # récupère les pages "citeuses" et supprime celles qui sont citées -> pour garder les pages juste "citeuses"

    # ETAPES :
    def steps(self):

        # JOB1 : initialisation du Page Rank
        # JOB2 : itérations
        return [MRStep(mapper=self.mapper1, reducer=self.reducer1)] + [MRStep(mapper=self.mapper2, combiner=self.combiner, reducer=self.reducer2)] * 10

        # calcul de la somme finale (il faut mettre en commentaire le return precedent pour activer celui-ci)
        #return [MRStep(mapper=self.mapper1,reducer=self.reducer1)]+[MRStep(mapper=self.mapper2,combiner=self.combiner,reducer=self.reducer2)]*10+[MRStep(mapper=self.mapper3,reducer=self.reducer3)]

    # JOB1 : initialisation du Page Rank

    # cette fonction map récupère la page i et la page j sur chaque ligne où i -> j (i cite j)
    # elle crée également une liste avec toutes les pages

    def mapper1(self, _, line):
        for elem in motif.findall(line):
            # calcul du nombre de pages
            if elem[0] not in PageRank.liste_nb_page:
                PageRank.liste_nb_page.append(elem[0])
                PageRank.pages_citeuses.append(elem[0])  # ajout de la page citeuse
            if elem[1] not in PageRank.liste_nb_page:
                PageRank.liste_nb_page.append(elem[1])
            yield elem[0], elem[1]


    # cette fonction reduce renvoie la page i avec son page rank initial
    # elle crée également le dico avec en clé la page i et en valeurs les pages j qu'elles citent

    def reducer1(self, page_i, val1):

        liens_sortant = list(val1)
        PageRank.nb_pages = len(PageRank.liste_nb_page)

        # dico avec en clé la page source (=la page i) et en valeurs les
        # liens sortants : les pages j que la page i cite (i -> j)
        if page_i not in PageRank.dico or PageRank.dico[page_i] == None:
            PageRank.dico[page_i] = liens_sortant
        else:
            PageRank.dico[page_i].extend(liens_sortant)

        # exception pour traiter les pages juste citées ou les pages "citeuses"
        for l in liens_sortant:
            # suppression des pages citées dans les pages citeuses
            if l in PageRank.pages_citeuses:
                PageRank.pages_citeuses.remove(l)
            if l not in PageRank.dico:
                PageRank.dico[l] = None

        yield page_i, 1 / PageRank.nb_pages


    # JOB2 :

    # redonne la page i, et les différents pages ranks associés aux
    # pages j -> i il reste à faire la somme de ces pages ranks

    def mapper2(self, key, PR):
        if PageRank.dico[key] != None:
            for page_i in PageRank.dico[key]:  # pages i de la somme
                yield key, (((1 - PageRank.c) * PR / sum(collections.Counter(PageRank.dico[key]).values()),
                             page_i))  # key = page j (les pages j de la somme j -> i)


    def combiner(self, key, valu4):
        # exception des pages justes "citeuses"
        if key in PageRank.pages_citeuses:
            yield key, 0  # on renvoie 0 pour que reduce prenne en compte cette page
        for val in valu4:
            PR, page_i = val
            yield page_i, PR  # key=page j


    def reducer2(self, page_i, val5):
        yield page_i, (PageRank.c * 1 / PageRank.nb_pages) + sum(val5)


    # fait la somme
    def mapper3(self, _, value):
        yield None, value

    def reducer3(self, _, value):
        yield None, sum(value)


if __name__ == '__main__':
    PageRank.run()