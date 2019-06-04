-- phpMyAdmin SQL Dump
-- version 4.8.4
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Jun 04, 2019 at 05:44 PM
-- Server version: 10.1.37-MariaDB
-- PHP Version: 7.1.26

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `libsys`
--

-- --------------------------------------------------------

--
-- Table structure for table `books`
--

CREATE TABLE `books` (
  `bookId` varchar(100) NOT NULL,
  `userId` varchar(100) NOT NULL,
  `issuedOn` varchar(100) DEFAULT '2018-01-01 00:00:00',
  `issuedBy` varchar(100) NOT NULL,
  `count` int(11) NOT NULL,
  `available` int(11) NOT NULL,
  `name` varchar(100) NOT NULL,
  `validTo` varchar(100) NOT NULL DEFAULT '2018-01-01 00:00:00',
  `returnStatus` int(11) NOT NULL DEFAULT '-1'
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `books`
--

INSERT INTO `books` (`bookId`, `userId`, `issuedOn`, `issuedBy`, `count`, `available`, `name`, `validTo`, `returnStatus`) VALUES
('1', 'Member1', '06-04-2019', 'Admin', 1, -1, '1', '06-11-2019', 0),
('2', '', '01-01-2018 00:00:00', '', 2, 3, '2', '01-01-2018 00:00:00', 1),
('3', '', '2018-01-01 00:00:00', '', 100, 100, 'testing', '2018-01-01 00:00:00', -1),
('4', '', '2018-01-01 00:00:00', '', 12, 12, 'Test', '2018-01-01 00:00:00', -1);

-- --------------------------------------------------------

--
-- Table structure for table `bookuser`
--

CREATE TABLE `bookuser` (
  `bookId` varchar(100) NOT NULL,
  `userId` varchar(100) NOT NULL,
  `issuedOn` varchar(100) NOT NULL,
  `returnedOn` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `bookuser`
--

INSERT INTO `bookuser` (`bookId`, `userId`, `issuedOn`, `returnedOn`) VALUES
('1', 'Member1', '06-04-2019', '06-04-2019');

-- --------------------------------------------------------

--
-- Table structure for table `person`
--

CREATE TABLE `person` (
  `userId` varchar(100) NOT NULL,
  `password` varchar(100) NOT NULL,
  `isAdmin` tinyint(1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `person`
--

INSERT INTO `person` (`userId`, `password`, `isAdmin`) VALUES
('Apurv', '317210366222432259211830225165226852401031495435200179136180691581924912021520070244', 1),
('Member1', '2157924023814116318512810724200119219242155189229119121622821821516316737015235130232241', 0),
('Member2', '2157924023814116318512810724200119219242155189229119121622821821516316737015235130232241', 0),
('Member3', '2157924023814116318512810724200119219242155189229119121622821821516316737015235130232241', 0);

--
-- Indexes for dumped tables
--

--
-- Indexes for table `books`
--
ALTER TABLE `books`
  ADD PRIMARY KEY (`bookId`);

--
-- Indexes for table `bookuser`
--
ALTER TABLE `bookuser`
  ADD PRIMARY KEY (`bookId`,`userId`,`issuedOn`);

--
-- Indexes for table `person`
--
ALTER TABLE `person`
  ADD PRIMARY KEY (`userId`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
